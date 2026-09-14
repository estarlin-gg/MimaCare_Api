using Microsoft.AspNetCore.Http;
using MimaCare.Application.Common.Exceptions;
using MimaCare.Application.Dtos.Branch;
using MimaCare.Application.Interface;
using MimaCare.Domain.Entities;
using MimaCare.Domain.Interfaces;
using MimaCare.Infrastructure.Repositories.Abstraction;

namespace MimaCare.Application.Services
{
    public class BranchService : IBranchService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IBranchRepository _branchRepository;
        private readonly IClinicRepository _clinicRepository;
        private readonly IDoctorRepository _doctorRepository;
        private readonly IDoctorBranchRepository _doctorBranchRepository;
        private readonly IUnitOfWork _unitOfWork;

        public BranchService(
            IHttpContextAccessor httpContextAccessor,
            IBranchRepository branchRepository,
            IClinicRepository clinicRepository,
            IDoctorRepository doctorRepository,
            IDoctorBranchRepository doctorBranchRepository,
            IUnitOfWork unitOfWork)
        {
            _httpContextAccessor = httpContextAccessor;
            _branchRepository = branchRepository;
            _clinicRepository = clinicRepository;
            _doctorRepository = doctorRepository;
            _doctorBranchRepository = doctorBranchRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<BranchDto>> GetMyBranchesAsync()
        {
            var clinicId = GetClinicId();
            var branches = await _branchRepository.GetByClinicAsync(clinicId);
            return branches.Select(b => ToDto(b)).ToList();
        }

        public async Task<BranchDto> CreateBranchAsync(CreateBranchDto dto)
        {
            var clinicId = GetClinicId();
            var clinic = await _clinicRepository.GetByIdWithPlanAsync(clinicId)
                ?? throw new NotFoundException("Clinic not found.");

            if (clinic.Plan == null)
                throw new ForbiddenException("La clínica no tiene un plan de suscripción asignado.");

            if (clinic.Plan.MaxBranches != null)
            {
                var current = await _branchRepository.CountByClinicAsync(clinicId);
                if (current >= clinic.Plan.MaxBranches.Value)
                {
                    throw new ForbiddenException(
                        $"Tu plan {clinic.Plan.Name} permite hasta {clinic.Plan.MaxBranches.Value} sucursal(es). " +
                        "Mejora tu plan para agregar más.");
                }
            }

            var branch = new Branch
            {
                Id = Guid.NewGuid(),
                ClinicId = clinicId,
                Name = dto.Name.Trim(),
                Address = string.IsNullOrWhiteSpace(dto.Address) ? null : dto.Address.Trim(),
                Phone = string.IsNullOrWhiteSpace(dto.Phone) ? null : dto.Phone.Trim()
            };

            await _branchRepository.CreateAsync(branch);
            await _unitOfWork.CommitAsync();

            return ToDto(branch);
        }

        public async Task<BranchDto> UpdateBranchAsync(Guid branchId, UpdateBranchDto dto)
        {
            var clinicId = GetClinicId();
            var branch = await GetOwnedBranchAsync(branchId, clinicId);

            branch.Name = dto.Name.Trim();
            branch.Address = string.IsNullOrWhiteSpace(dto.Address) ? null : dto.Address.Trim();
            branch.Phone = string.IsNullOrWhiteSpace(dto.Phone) ? null : dto.Phone.Trim();
            branch.IsActive = dto.IsActive;

            await _branchRepository.UpdateAsync(branch);
            await _unitOfWork.CommitAsync();

            return ToDto(branch);
        }

        public async Task<string> DeleteBranchAsync(Guid branchId)
        {
            var clinicId = GetClinicId();
            var branch = await GetOwnedBranchAsync(branchId, clinicId);

            if (await _branchRepository.HasAppointmentsAsync(branchId))
                throw new ConflictException("No se puede eliminar una sucursal con citas asociadas.");

            await _branchRepository.DeleteAsync(branch);
            await _unitOfWork.CommitAsync();

            return "Sucursal eliminada correctamente.";
        }

        public async Task<IEnumerable<BranchDto>> GetDoctorBranchesAsync(Guid doctorId)
        {
            var clinicId = GetClinicId();
            var doctor = await _doctorRepository.GetDoctorById(doctorId);
            if (doctor == null || doctor.ClinicId != clinicId)
                throw new NotFoundException("Doctor not found in this clinic.");

            var branches = await _doctorBranchRepository.GetBranchesByDoctorAsync(doctorId);
            return branches.Select(b => ToDto(b)).ToList();
        }

        public async Task<IEnumerable<Guid>> GetDoctorBranchIdsAsync(Guid doctorId)
        {
            var clinicId = GetClinicId();
            var doctor = await _doctorRepository.GetDoctorById(doctorId);
            if (doctor == null || doctor.ClinicId != clinicId)
                throw new NotFoundException("Doctor not found in this clinic.");

            return await _doctorBranchRepository.GetBranchIdsByDoctorAsync(doctorId);
        }

        public async Task<string> SetDoctorBranchesAsync(Guid doctorId, AssignDoctorBranchesDto dto)
        {
            var clinicId = GetClinicId();
            var doctor = await _doctorRepository.GetDoctorById(doctorId)
                ?? throw new NotFoundException("Doctor not found.");
            if (doctor.ClinicId != clinicId)
                throw new NotFoundException("Doctor not found in this clinic.");

            if (dto.BranchIds.Any())
            {
                var clinicBranches = (await _branchRepository.GetByClinicAsync(clinicId))
                    .Select(b => b.Id)
                    .ToHashSet();

                var invalid = dto.BranchIds.Where(id => !clinicBranches.Contains(id)).ToList();
                if (invalid.Any())
                    throw new BadRequestException("Una o más sucursales no pertenecen a la clínica.");
            }

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                await _doctorBranchRepository.DeleteAllForDoctorAsync(doctorId);

                foreach (var branchId in dto.BranchIds)
                {
                    await _doctorBranchRepository.CreateAsync(new DoctorBranch
                    {
                        DoctorId = doctorId,
                        BranchId = branchId
                    });
                }

                await _unitOfWork.CommitAsync();
                return "Sucursales del doctor actualizadas correctamente.";
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        private async Task<Branch> GetOwnedBranchAsync(Guid branchId, Guid clinicId)
        {
            var branch = await _branchRepository.GetByIdAsync(branchId)
                ?? throw new NotFoundException("Branch not found.");
            if (branch.ClinicId != clinicId)
                throw new NotFoundException("Branch not found in this clinic.");
            return branch;
        }

        private Guid GetClinicId()
        {
            var claim = _httpContextAccessor.HttpContext?.User?.FindFirst("clinicId")?.Value;
            if (string.IsNullOrEmpty(claim) || !Guid.TryParse(claim, out var clinicId))
                throw new UnauthorizedException("Clinic not found.");
            return clinicId;
        }

        private static BranchDto ToDto(Branch b)
        {
            return new BranchDto
            {
                Id = b.Id,
                ClinicId = b.ClinicId,
                Name = b.Name,
                Address = b.Address,
                Phone = b.Phone,
                IsActive = b.IsActive,
                CreatedAt = b.CreatedAt
            };
        }
    }
}