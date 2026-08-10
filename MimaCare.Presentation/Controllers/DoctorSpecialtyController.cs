using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MimaCare.Application.Dtos.Specialty;
using MimaCare.Application.Interface;
using MimaCare.Presentation.Common;

namespace MimaCare.Presentation.Controllers
{
    [Route("api/doctors/{doctorId:guid}/specialties")]
    [ApiController]
    [Authorize]
    public class DoctorSpecialtyController : BaseController
    {
        private readonly ISpecialtyService _specialtyService;

        public DoctorSpecialtyController(ISpecialtyService specialtyService)
        {
            _specialtyService = specialtyService;
        }

        [HttpGet]
        public async Task<IActionResult> GetDoctorSpecialties(Guid doctorId)
        {
            var specialties = await _specialtyService.GetDoctorSpecialties(doctorId);
            return Success(specialties);
        }

        [HttpPut]
        [Authorize(Roles = "Clinic,Super")]
        public async Task<IActionResult> AssignDoctorSpecialties(Guid doctorId, AssignDoctorSpecialtiesDto dto)
        {
            var result = await _specialtyService.AssignDoctorSpecialties(doctorId, dto);
            return Success(result, "Specialties assigned successfully.");
        }
    }
}
