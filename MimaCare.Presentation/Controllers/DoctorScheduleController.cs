using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MimaCare.Application.Dtos.DoctorSchedule;
using MimaCare.Application.Interface;
using MimaCare.Presentation.Common;

namespace MimaCare.Presentation.Controllers
{
    [Route("api/doctors/{doctorId:guid}/schedule")]
    [ApiController]
    [Authorize]
    public class DoctorScheduleController : BaseController
    {
        private readonly IDoctorScheduleService _scheduleService;

        public DoctorScheduleController(IDoctorScheduleService scheduleService)
        {
            _scheduleService = scheduleService;
        }

        [HttpGet]
        public async Task<IActionResult> GetSchedule(Guid doctorId)
        {
            var schedules = await _scheduleService.GetSchedulesByDoctor(doctorId);
            return Success(schedules);
        }

        [HttpPut]
        [Authorize(Roles = "Clinic,Super")]
        public async Task<IActionResult> SetSchedule(Guid doctorId, List<CreateDoctorScheduleDto> schedules)
        {
            await _scheduleService.SetDoctorSchedule(doctorId, schedules);
            return Success("Schedule updated successfully.");
        }

        [HttpDelete]
        [Authorize(Roles = "Clinic,Super")]
        public async Task<IActionResult> ClearSchedule(Guid doctorId)
        {
            await _scheduleService.ClearDoctorSchedule(doctorId);
            return Success("Schedule cleared. Doctor is now on open schedule.");
        }
    }
}
