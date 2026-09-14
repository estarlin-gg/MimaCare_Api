using Microsoft.AspNetCore.Http;
using MimaCare.Application.Common.Exceptions;
using MimaCare.Application.Dtos.SubscriptionPlan;
using MimaCare.Application.Interface;
using MimaCare.Domain.Entities;
using MimaCare.Domain.Interfaces;
using MimaCare.Infrastructure.Repositories.Abstraction;

namespace MimaCare.Application.Services
{
    public class SubscriptionPlanService : ISubscriptionPlanService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISubscriptionPlanRepository _planRepository;
        private readonly IClinicRepository _clinicRepository;
        private readonly IBranchRepository _branchRepository;
        private readonly IDoctorRepository _doctorRepository;
        private readonly IUnitOfWork _unitOfWork;

        public SubscriptionPlanService(
            IHttpContextAccessor httpContextAccessor,
            ISubscriptionPlanRepository planRepository,
            IClinicRepository clinicRepository,
            IBranchRepository branchRepository,
            IDoctorRepository doctorRepository,
            IUnitOfWork unitOfWork)
        {
            _httpContextAccessor = httpContextAccessor;
            _planRepository = planRepository;
            _clinicRepository = clinicRepository;
            _branchRepository = branchRepository;
            _doctorRepository = doctorRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<SubscriptionPlanDto>> GetAllPlansAsync()
        {
            var plans = await _planRepository.GetAllActiveAsync();
            return plans.Select(p => ToDto(p)).ToList();
        }

        public async Task<PlanUsageDto> GetMyPlanUsageAsync()
        {
            var clinicId = GetClinicId();
            return await BuildUsageAsync(clinicId);
        }

        public async Task<PlanUsageDto> GetClinicPlanUsageAsync(Guid clinicId)
        {
            var clinic = await _clinicRepository.GetByIdWithPlanAsync(clinicId)
                ?? throw new NotFoundException("Clinic not found.");
            return await BuildUsageAsync(clinicId);
        }

        public async Task<string> CreatePlanAsync(CreateSubscriptionPlanDto dto)
        {
            var existing = await _planRepository.GetByNameAsync(dto.Name.Trim());
            if (existing != null)
                throw new ConflictException("Ya existe un plan con ese nombre.");

            var plan = new SubscriptionPlan
            {
                Id = Guid.NewGuid(),
                Name = dto.Name.Trim(),
                Description = string.IsNullOrWhiteSpace(dto.Description) ? null : dto.Description.Trim(),
                MaxBranches = dto.MaxBranches,
                MaxDoctors = dto.MaxDoctors,
                AllowsUnlimitedAppointments = dto.AllowsUnlimitedAppointments,
                IsActive = true
            };

            await _planRepository.CreateAsync(plan);
            await _unitOfWork.CommitAsync();

            return "Plan creado correctamente.";
        }

        public async Task<string> UpdatePlanAsync(Guid planId, UpdateSubscriptionPlanDto dto)
        {
            var plan = await _planRepository.GetByIdAsync(planId)
                ?? throw new NotFoundException("Plan not found.");

            var duplicate = await _planRepository.GetByNameAsync(dto.Name.Trim());
            if (duplicate != null && duplicate.Id != plan.Id)
                throw new ConflictException("Ya existe un plan con ese nombre.");

            plan.Name = dto.Name.Trim();
            plan.Description = string.IsNullOrWhiteSpace(dto.Description) ? null : dto.Description.Trim();
            plan.MaxBranches = dto.MaxBranches;
            plan.MaxDoctors = dto.MaxDoctors;
            plan.AllowsUnlimitedAppointments = dto.AllowsUnlimitedAppointments;
            plan.IsActive = dto.IsActive;

            await _unitOfWork.CommitAsync();

            return "Plan actualizado correctamente.";
        }

        public async Task<string> AssignPlanToClinicAsync(Guid clinicId, AssignPlanToClinicDto dto)
        {
            var clinic = await _clinicRepository.GetByIdAsync(clinicId)
                ?? throw new NotFoundException("Clinic not found.");

            var plan = await _planRepository.GetByIdAsync(dto.PlanId)
                ?? throw new NotFoundException("Plan not found.");

            clinic.PlanId = plan.Id;
            await _unitOfWork.CommitAsync();

            return $"Plan {plan.Name} asignado a la clínica correctamente.";
        }

        private async Task<PlanUsageDto> BuildUsageAsync(Guid clinicId)
        {
            var clinic = await _clinicRepository.GetByIdWithPlanAsync(clinicId)
                ?? throw new NotFoundException("Clinic not found.");

            var branchCount = await _branchRepository.CountByClinicAsync(clinicId);
            var doctorCount = await _doctorRepository.CountByClinicAsync(clinicId);

            return new PlanUsageDto
            {
                Plan = clinic.Plan == null ? null : ToDto(clinic.Plan),
                BranchCount = branchCount,
                DoctorCount = doctorCount,
                CanCreateBranch = clinic.Plan == null || clinic.Plan.MaxBranches == null
                                    || branchCount < clinic.Plan.MaxBranches.Value,
                CanCreateDoctor = clinic.Plan == null || clinic.Plan.MaxDoctors == null
                                    || doctorCount < clinic.Plan.MaxDoctors.Value
            };
        }

        private Guid GetClinicId()
        {
            var claim = _httpContextAccessor.HttpContext?.User?.FindFirst("clinicId")?.Value;
            if (string.IsNullOrEmpty(claim) || !Guid.TryParse(claim, out var clinicId))
                throw new UnauthorizedException("Clinic not found.");
            return clinicId;
        }

        private static SubscriptionPlanDto ToDto(SubscriptionPlan p)
        {
            return new SubscriptionPlanDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                MaxBranches = p.MaxBranches,
                MaxDoctors = p.MaxDoctors,
                AllowsUnlimitedAppointments = p.AllowsUnlimitedAppointments,
                IsActive = p.IsActive
            };
        }
    }
}