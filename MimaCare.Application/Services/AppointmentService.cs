using Mapster;
using Microsoft.AspNetCore.Http;
using MimaCare.Application.Common;
using MimaCare.Application.Common.Exceptions;
using MimaCare.Application.Dtos.Appointment;
using MimaCare.Application.Dtos.Common;
using MimaCare.Application.Interface;
using MimaCare.Domain.Common;
using MimaCare.Domain.Entities;
using MimaCare.Domain.Enums;
using MimaCare.Domain.Interfaces;
using MimaCare.Infrastructure.Constants;
using MimaCare.Infrastructure.Repositories.Abstraction;

namespace MimaCare.Application.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IDoctorRepository _doctorRepository;
        private readonly IPatientRepository _patientRepository;
        private readonly IDoctorScheduleRepository _scheduleRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUnitOfWork _unitOfWork;

        public AppointmentService(
            IAppointmentRepository appointmentRepository,
            IDoctorRepository doctorRepository,
            IPatientRepository patientRepository,
            IDoctorScheduleRepository scheduleRepository,
            IHttpContextAccessor httpContextAccessor,
            IUnitOfWork unitOfWork)
        {
            _appointmentRepository = appointmentRepository;
            _doctorRepository = doctorRepository;
            _patientRepository = patientRepository;
            _scheduleRepository = scheduleRepository;
            _httpContextAccessor = httpContextAccessor;
            _unitOfWork = unitOfWork;
        }

        public async Task<AppointmentDto> CreateAppointment(CreateAppointmentDto dto)
        {
            var clinicId = GetClinicId();
            var userId = GetUserId();
            var roles = GetUserRoles();

            // Validate patient belongs to clinic
            var patient = await _patientRepository.GetPatientById(dto.PatientId, clinicId);
            if (patient == null)
                throw new NotFoundException("Patient not found in this clinic.");

            // Determine doctor: Clinic assigns, Doctor auto-assigns
            Guid doctorId;
            if (roles.Contains(Roles.Clinic) || roles.Contains(Roles.Admin))
            {
                if (dto.DoctorId == Guid.Empty)
                    throw new BadRequestException("Doctor is required when creating an appointment.");
                doctorId = dto.DoctorId;
            }
            else if (roles.Contains(Roles.Doctor))
            {
                doctorId = Guid.Parse(userId!);
            }
            else
            {
                throw new ForbiddenException("You do not have permission to create appointments.");
            }

            // Validate doctor belongs to clinic.
            // Doctors act on their own profile via the JWT user id;
            // clinics pass the doctor entity id in the DTO.
            var doctorEntity = roles.Contains(Roles.Doctor)
                ? await _doctorRepository.GetDoctorByUserId(Guid.Parse(userId!))
                : await _doctorRepository.GetDoctorById(doctorId);

            if (doctorEntity == null || doctorEntity.ClinicId != clinicId)
                throw new NotFoundException("Doctor not found in this clinic.");

            // Validate schedule if exists (open schedule = no restriction)
            await ValidateSchedule(doctorEntity.Id, dto.AppointmentDate, dto.StartTime, dto.EndTime);

            // Check for overlapping appointments
            var existingAppointments = await _appointmentRepository.GetAppointmentsByDoctorId(doctorEntity.Id);
            var overlapping = existingAppointments.Any(a =>
                a.AppointmentDate == dto.AppointmentDate &&
                a.Status != AppointmentStatus.Cancelled &&
                a.Status != AppointmentStatus.Rejected &&
                ((dto.StartTime >= a.StartTime && dto.StartTime < a.EndTime) ||
                 (dto.EndTime > a.StartTime && dto.EndTime <= a.EndTime) ||
                 (dto.StartTime <= a.StartTime && dto.EndTime >= a.EndTime)));
            if (overlapping)
                throw new ConflictException("The doctor already has an appointment in this time slot.");

            var appointment = dto.Adapt<Appointment>();
            appointment.ClinicId = clinicId;
            appointment.DoctorId = doctorEntity.Id;
            appointment.Status = AppointmentStatus.Pending;

            await _appointmentRepository.CreateAppointment(appointment);
            await _unitOfWork.CommitAsync();

            return await GetFullAppointment(appointment.Id);
        }

        public async Task<AppointmentDto> GetAppointmentById(Guid id)
        {
            var clinicId = GetClinicId();
            var appointment = await _appointmentRepository.GetAppointmentById(id);
            if (appointment == null || appointment.ClinicId != clinicId)
                throw new NotFoundException("Appointment not found.");

            return appointment.Adapt<AppointmentDto>();
        }

        public async Task<PagedResult<AppointmentDto>> GetAppointmentsByClinic(PaginationParams pagination)
        {
            var clinicId = GetClinicId();
            var appointments = await _appointmentRepository.GetAppointmentsByClinicId(
                clinicId,
                pagination.EffectivePage,
                pagination.EffectivePageSize);
            return appointments.AdaptTo<Appointment, AppointmentDto>();
        }

        public async Task<PagedResult<AppointmentDto>> GetMyAppointments(PaginationParams pagination)
        {
            var roles = GetUserRoles();
            var userId = GetUserId();

            if (roles.Contains(Roles.Doctor))
            {
                var doctor = await _doctorRepository.GetDoctorByUserId(Guid.Parse(userId!));
                if (doctor == null)
                    throw new UnauthorizedException("Doctor profile not found.");
                var appointments = await _appointmentRepository.GetAppointmentsByDoctorId(
                    doctor.Id,
                    pagination.EffectivePage,
                    pagination.EffectivePageSize);
                return appointments.AdaptTo<Appointment, AppointmentDto>();
            }

            return await GetAppointmentsByClinic(pagination);
        }

        public async Task<PagedResult<AppointmentDto>> GetAppointmentsByDoctorId(Guid doctorId, PaginationParams pagination)
        {
            var clinicId = GetClinicId();
            var doctor = await _doctorRepository.GetDoctorById(doctorId);
            if (doctor == null || doctor.ClinicId != clinicId)
                throw new NotFoundException("Doctor not found in this clinic.");

            var appointments = await _appointmentRepository.GetAppointmentsByDoctorId(
                doctor.Id,
                pagination.EffectivePage,
                pagination.EffectivePageSize);
            return appointments.AdaptTo<Appointment, AppointmentDto>();
        }

        public async Task<PagedResult<AppointmentDto>> GetAppointmentsByPatientId(Guid patientId, PaginationParams pagination)
        {
            var clinicId = GetClinicId();
            var patient = await _patientRepository.GetPatientById(patientId, clinicId);
            if (patient == null)
                throw new NotFoundException("Patient not found in this clinic.");

            var appointments = await _appointmentRepository.GetAppointmentsByPatientId(
                patientId,
                pagination.EffectivePage,
                pagination.EffectivePageSize);
            return appointments.AdaptTo<Appointment, AppointmentDto>();
        }

        public async Task<AppointmentDto> UpdateAppointmentStatus(Guid id, UpdateAppointmentStatusDto dto)
        {
            var clinicId = GetClinicId();
            var roles = GetUserRoles();
            var appointment = await _appointmentRepository.GetAppointmentById(id);
            if (appointment == null || appointment.ClinicId != clinicId)
                throw new NotFoundException("Appointment not found.");

            if (roles.Contains(Roles.Doctor))
            {
                var userId = GetUserId();
                var doctor = await _doctorRepository.GetDoctorByUserId(Guid.Parse(userId!));
                if (doctor == null || appointment.DoctorId != doctor.Id)
                    throw new ForbiddenException("You can only update your own appointments.");
            }

            appointment.Status = dto.Status;
            await _unitOfWork.CommitAsync();

            return await GetFullAppointment(appointment.Id);
        }

        public async Task<AppointmentDto> UpdateAppointment(Guid id, UpdateAppointmentDto dto)
        {
            var clinicId = GetClinicId();
            var roles = GetUserRoles();

            var appointment = await _appointmentRepository.GetAppointmentById(id);
            if (appointment == null || appointment.ClinicId != clinicId)
                throw new NotFoundException("Appointment not found.");

            if (roles.Contains(Roles.Doctor))
            {
                var userId = GetUserId();
                var doctor = await _doctorRepository.GetDoctorByUserId(Guid.Parse(userId!));
                if (doctor == null || appointment.DoctorId != doctor.Id)
                    throw new ForbiddenException("You can only edit your own appointments.");
            }

            // Validate patient belongs to clinic
            var patient = await _patientRepository.GetPatientById(dto.PatientId, clinicId);
            if (patient == null)
                throw new NotFoundException("Patient not found in this clinic.");

            // Validate schedule if exists
            await ValidateSchedule(appointment.DoctorId, dto.AppointmentDate, dto.StartTime, dto.EndTime);

            // Check for overlapping appointments (excluding the current one)
            var existingAppointments = await _appointmentRepository.GetAppointmentsByDoctorId(appointment.DoctorId);
            var overlapping = existingAppointments.Any(a =>
                a.Id != appointment.Id &&
                a.AppointmentDate == dto.AppointmentDate &&
                a.Status != AppointmentStatus.Cancelled &&
                a.Status != AppointmentStatus.Rejected &&
                ((dto.StartTime >= a.StartTime && dto.StartTime < a.EndTime) ||
                 (dto.EndTime > a.StartTime && dto.EndTime <= a.EndTime) ||
                 (dto.StartTime <= a.StartTime && dto.EndTime >= a.EndTime)));
            if (overlapping)
                throw new ConflictException("The doctor already has an appointment in this time slot.");

            appointment.PatientId = dto.PatientId;
            appointment.AppointmentDate = dto.AppointmentDate;
            appointment.StartTime = dto.StartTime;
            appointment.EndTime = dto.EndTime;
            appointment.Notes = dto.Notes;

            await _unitOfWork.CommitAsync();

            return await GetFullAppointment(appointment.Id);
        }

        public async Task<AppointmentDto> AssignDoctor(Guid id, AssignDoctorDto dto)
        {
            var clinicId = GetClinicId();
            var roles = GetUserRoles();
            if (!roles.Contains(Roles.Clinic) && !roles.Contains(Roles.Admin))
                throw new ForbiddenException("Only clinic admins can assign doctors.");

            var appointment = await _appointmentRepository.GetAppointmentById(id);
            if (appointment == null || appointment.ClinicId != clinicId)
                throw new NotFoundException("Appointment not found.");

            var doctor = await _doctorRepository.GetDoctorById(dto.DoctorId);
            if (doctor == null || doctor.ClinicId != clinicId)
                throw new NotFoundException("Doctor not found in this clinic.");

            // Validate schedule if exists
            await ValidateSchedule(doctor.Id, appointment.AppointmentDate,
                appointment.StartTime ?? TimeOnly.MinValue, appointment.EndTime ?? TimeOnly.MinValue);

            appointment.DoctorId = doctor.Id;
            await _unitOfWork.CommitAsync();

            return await GetFullAppointment(appointment.Id);
        }

        private async Task<AppointmentDto> GetFullAppointment(Guid id)
        {
            var appointment = await _appointmentRepository.GetAppointmentById(id);
            return appointment.Adapt<AppointmentDto>();
        }

        private async Task ValidateSchedule(Guid doctorId, DateOnly date, TimeOnly startTime, TimeOnly endTime)
        {
            var schedules = await _scheduleRepository.GetSchedulesByDoctorId(doctorId);
            if (!schedules.Any())
                return; // Open schedule - no restrictions

            var daySchedule = schedules.FirstOrDefault(s =>
                s.DayOfWeek == date.DayOfWeek && s.IsActive);
            if (daySchedule == null)
                throw new BadRequestException(
                    $"The doctor does not work on {date.DayOfWeek}. " +
                    "Please select another date.");

            if (startTime < daySchedule.StartTime)
                throw new BadRequestException(
                    $"The doctor's schedule starts at {daySchedule.StartTime}. " +
                    "Please select a later time.");

            if (endTime > daySchedule.EndTime)
                throw new BadRequestException(
                    $"The doctor's schedule ends at {daySchedule.EndTime}. " +
                    "Please select an earlier time.");
        }

        private Guid GetClinicId()
        {
            var claim = _httpContextAccessor.HttpContext?.User?.FindFirst("clinicId")?.Value;
            if (string.IsNullOrEmpty(claim) || !Guid.TryParse(claim, out var clinicId))
                throw new UnauthorizedException("Clinic not found.");
            return clinicId;
        }

        private string GetUserId()
        {
            var claim = _httpContextAccessor.HttpContext?.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(claim))
                throw new UnauthorizedException("User not found.");
            return claim;
        }

        private List<string> GetUserRoles()
        {
            var claims = _httpContextAccessor.HttpContext?.User?.FindAll(System.Security.Claims.ClaimTypes.Role);
            return claims?.Select(c => c.Value).ToList() ?? new List<string>();
        }
    }
}
