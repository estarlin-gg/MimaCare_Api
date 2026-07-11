using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MimaCare.Application.Dtos.Appointment;
using MimaCare.Application.Interface;
using MimaCare.Infrastructure.Constants;
using MimaCare.Presentation.Common;

namespace MimaCare.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AppointmentController : BaseController
    {
        private readonly IAppointmentService _appointmentService;

        public AppointmentController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAppointments()
        {
            var appointments = await _appointmentService.GetAppointmentsByClinic();
            return Success(appointments);
        }

        [HttpGet("my")]
        public async Task<IActionResult> GetMyAppointments()
        {
            var appointments = await _appointmentService.GetMyAppointments();
            return Success(appointments);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetAppointmentById(Guid id)
        {
            var appointment = await _appointmentService.GetAppointmentById(id);
            return Success(appointment);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAppointment(CreateAppointmentDto dto)
        {
            var appointment = await _appointmentService.CreateAppointment(dto);
            return CreatedResponse(appointment, "Appointment created successfully.");
        }

        [HttpPut("{id:guid}/status")]
        public async Task<IActionResult> UpdateStatus(Guid id, UpdateAppointmentStatusDto dto)
        {
            var appointment = await _appointmentService.UpdateAppointmentStatus(id, dto);
            return Success(appointment, "Appointment status updated.");
        }

        [HttpPut("{id:guid}/assign-doctor")]
        [Authorize(Roles = "Clinic,Super")]
        public async Task<IActionResult> AssignDoctor(Guid id, AssignDoctorDto dto)
        {
            var appointment = await _appointmentService.AssignDoctor(id, dto);
            return Success(appointment, "Doctor assigned successfully.");
        }

        [HttpGet("by-doctor/{doctorId:guid}")]
        public async Task<IActionResult> GetByDoctor(Guid doctorId)
        {
            var appointments = await _appointmentService.GetAppointmentsByDoctorId(doctorId);
            return Success(appointments);
        }

        [HttpGet("by-patient/{patientId:guid}")]
        public async Task<IActionResult> GetByPatient(Guid patientId)
        {
            var appointments = await _appointmentService.GetAppointmentsByPatientId(patientId);
            return Success(appointments);
        }
    }
}
