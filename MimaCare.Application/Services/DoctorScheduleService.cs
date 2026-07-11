using Mapster;
using Microsoft.AspNetCore.Http;
using MimaCare.Application.Common.Exceptions;
using MimaCare.Application.Dtos.DoctorSchedule;
using MimaCare.Application.Interface;
using MimaCare.Domain.Entities;
using MimaCare.Domain.Interfaces;
using MimaCare.Infrastructure.Constants;
using MimaCare.Infrastructure.Repositories.Abstraction;

namespace MimaCare.Application.Services
{
    public class DoctorScheduleService : IDoctorScheduleService
    {
        private readonly IDoctorScheduleRepository _scheduleRepository;
        private readonly IDoctorRepository _doctorRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUnitOfWork _unitOfWork;

        public DoctorScheduleService(
            IDoctorScheduleRepository scheduleRepository,
            IDoctorRepository doctorRepository,
            IHttpContextAccessor httpContextAccessor,
            IUnitOfWork unitOfWork)
        {
            _scheduleRepository = scheduleRepository;
            _doctorRepository = doctorRepository;
            _httpContextAccessor = httpContextAccessor;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<DoctorScheduleDto>> GetSchedulesByDoctor(Guid doctorId)
        {
            var clinicId = GetClinicId();
            var doctor = await _doctorRepository.GetDoctorById(doctorId);
            if (doctor == null || doctor.ClinicId != clinicId)
                throw new NotFoundException("Doctor not found in this clinic.");

            var schedules = await _scheduleRepository.GetSchedulesByDoctorId(doctorId);
            return schedules.Adapt<IEnumerable<DoctorScheduleDto>>();
        }

        public async Task SetDoctorSchedule(Guid doctorId, IEnumerable<CreateDoctorScheduleDto> scheduleDtos)
        {
            var clinicId = GetClinicId();
            var roles = GetUserRoles();
            if (!roles.Contains(Roles.Clinic) && !roles.Contains(Roles.Admin))
                throw new ForbiddenException("Only clinic admins can set doctor schedules.");

            var doctor = await _doctorRepository.GetDoctorById(doctorId);
            if (doctor == null || doctor.ClinicId != clinicId)
                throw new NotFoundException("Doctor not found in this clinic.");

            // Remove existing schedules
            var existingSchedules = await _scheduleRepository.GetSchedulesByDoctorId(doctorId);
            foreach (var existing in existingSchedules)
                await _scheduleRepository.DeleteSchedule(existing);

            // Create new schedules
            foreach (var dto in scheduleDtos)
            {
                var schedule = dto.Adapt<DoctorSchedule>();
                schedule.DoctorId = doctorId;
                await _scheduleRepository.CreateSchedule(schedule);
            }

            await _scheduleRepository.SaveChangesAsync();
        }

        public async Task ClearDoctorSchedule(Guid doctorId)
        {
            var clinicId = GetClinicId();
            var roles = GetUserRoles();
            if (!roles.Contains(Roles.Clinic) && !roles.Contains(Roles.Admin))
                throw new ForbiddenException("Only clinic admins can clear doctor schedules.");

            var doctor = await _doctorRepository.GetDoctorById(doctorId);
            if (doctor == null || doctor.ClinicId != clinicId)
                throw new NotFoundException("Doctor not found in this clinic.");

            var existingSchedules = await _scheduleRepository.GetSchedulesByDoctorId(doctorId);
            foreach (var existing in existingSchedules)
                await _scheduleRepository.DeleteSchedule(existing);

            await _scheduleRepository.SaveChangesAsync();
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
