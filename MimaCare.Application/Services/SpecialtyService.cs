using Mapster;
using Microsoft.AspNetCore.Http;
using MimaCare.Application.Common.Exceptions;
using MimaCare.Application.Dtos.Specialty;
using MimaCare.Application.Interface;
using MimaCare.Domain.Entities;
using MimaCare.Domain.Interfaces;
using MimaCare.Infrastructure.Constants;
using MimaCare.Infrastructure.Repositories.Abstraction;

namespace MimaCare.Application.Services
{
    public class SpecialtyService : ISpecialtyService
    {
        private readonly ISpecialtyRepository _specialtyRepository;
        private readonly IDoctorRepository _doctorRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUnitOfWork _unitOfWork;

        public SpecialtyService(
            ISpecialtyRepository specialtyRepository,
            IDoctorRepository doctorRepository,
            IHttpContextAccessor httpContextAccessor,
            IUnitOfWork unitOfWork)
        {
            _specialtyRepository = specialtyRepository;
            _doctorRepository = doctorRepository;
            _httpContextAccessor = httpContextAccessor;
            _unitOfWork = unitOfWork;
        }

        public async Task<SpecialtyDto> CreateSpecialty(CreateSpecialtyDto dto)
        {
            var roles = GetUserRoles();
            if (!roles.Contains(Roles.Clinic) && !roles.Contains(Roles.Admin))
                throw new ForbiddenException("Only clinic admins can create specialties.");

            var specialty = dto.Adapt<Specialty>();

            await _specialtyRepository.CreateSpecialty(specialty);
            await _specialtyRepository.SaveChangesAsync();

            return specialty.Adapt<SpecialtyDto>();
        }

        public async Task<IEnumerable<SpecialtyDto>> GetAllSpecialties()
        {
            var specialties = await _specialtyRepository.GetAllSpecialties();
            return specialties.Adapt<IEnumerable<SpecialtyDto>>();
        }

        public async Task<SpecialtyDto> GetSpecialtyById(Guid id)
        {
            var specialty = await _specialtyRepository.GetSpecialtyById(id);
            if (specialty == null)
                throw new NotFoundException("Specialty not found.");

            return specialty.Adapt<SpecialtyDto>();
        }

        public async Task<string> UpdateSpecialty(Guid id, UpdateSpecialtyDto dto)
        {
            var roles = GetUserRoles();
            if (!roles.Contains(Roles.Clinic) && !roles.Contains(Roles.Admin))
                throw new ForbiddenException("Only clinic admins can update specialties.");

            var specialty = await _specialtyRepository.GetSpecialtyById(id);
            if (specialty == null)
                throw new NotFoundException("Specialty not found.");

            dto.Adapt(specialty);

            await _specialtyRepository.UpdateSpecialty(specialty);
            await _specialtyRepository.SaveChangesAsync();

            return "Specialty updated successfully.";
        }

        public async Task<string> DeleteSpecialty(Guid id)
        {
            var roles = GetUserRoles();
            if (!roles.Contains(Roles.Clinic) && !roles.Contains(Roles.Admin))
                throw new ForbiddenException("Only clinic admins can delete specialties.");

            var specialty = await _specialtyRepository.GetSpecialtyById(id);
            if (specialty == null)
                throw new NotFoundException("Specialty not found.");

            await _specialtyRepository.DeleteSpecialty(specialty);
            await _specialtyRepository.SaveChangesAsync();

            return "Specialty deleted successfully.";
        }

        public async Task<IEnumerable<SpecialtyDto>> GetDoctorSpecialties(Guid doctorId)
        {
            var clinicId = GetClinicId();

            var doctor = await _doctorRepository.GetDoctorById(doctorId);
            if (doctor == null || doctor.ClinicId != clinicId)
                throw new NotFoundException("Doctor not found in this clinic.");

            var specialties = await _specialtyRepository.GetSpecialtiesByDoctorId(doctor.Id);
            return specialties.Adapt<IEnumerable<SpecialtyDto>>();
        }

        public async Task<string> AssignDoctorSpecialties(Guid doctorId, AssignDoctorSpecialtiesDto dto)
        {
            var clinicId = GetClinicId();
            var roles = GetUserRoles();
            if (!roles.Contains(Roles.Clinic) && !roles.Contains(Roles.Admin))
                throw new ForbiddenException("Only clinic admins can assign specialties to doctors.");

            var doctor = await _doctorRepository.GetDoctorById(doctorId);
            if (doctor == null || doctor.ClinicId != clinicId)
                throw new NotFoundException("Doctor not found in this clinic.");

            var specialtyIds = dto.SpecialtyIds.Distinct().ToList();

            var validIds = (await _specialtyRepository.GetAllSpecialties())
                .Select(s => s.Id)
                .ToHashSet();

            var invalidIds = specialtyIds.Where(id => !validIds.Contains(id)).ToList();
            if (invalidIds.Any())
                throw new BadRequestException("One or more specialty ids are invalid.");

            var existingRelations = await _specialtyRepository.GetDoctorSpecialtyRelations(doctor.Id);
            foreach (var relation in existingRelations)
                await _specialtyRepository.DeleteDoctorSpecialty(relation);

            foreach (var specialtyId in specialtyIds)
            {
                await _specialtyRepository.AddDoctorSpecialty(new DoctorSpecialty
                {
                    DoctorId = doctor.Id,
                    SpecialtyId = specialtyId
                });
            }

            await _specialtyRepository.SaveChangesAsync();

            return "Specialties assigned successfully.";
        }

        private Guid GetClinicId()
        {
            var claim = _httpContextAccessor.HttpContext?.User?.FindFirst("clinicId")?.Value;
            if (string.IsNullOrEmpty(claim) || !Guid.TryParse(claim, out var clinicId))
                throw new UnauthorizedException("Clinic not found.");
            return clinicId;
        }

        private List<string> GetUserRoles()
        {
            var claims = _httpContextAccessor.HttpContext?.User?.FindAll(System.Security.Claims.ClaimTypes.Role);
            return claims?.Select(c => c.Value).ToList() ?? new List<string>();
        }
    }
}
