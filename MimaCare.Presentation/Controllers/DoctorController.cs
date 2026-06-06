using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MimaCare.Application.Dtos.Doctor;
using MimaCare.Application.Interface;
using MimaCare.Presentation.Common;

namespace MimaCare.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles ="Clinic,Super")]
    public class DoctorController : BaseController
    {
        private readonly IDoctorService _doctorService;
        public DoctorController(IDoctorService doctorService)
        {
            _doctorService = doctorService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllDoctors()
        {
            var doctors = await _doctorService.GetAllDoctor();
            return Success(doctors);
        }

        [HttpPost]
        public async Task<IActionResult> CreateDoctor(CreateDoctorDto dto)
        {
            var res = await _doctorService.CreateDoctor(dto);

            return CreatedResponse("Doctor creado con exito");
        }

        [HttpGet("{doctorId:guid}")]
        public async Task<IActionResult> GetDoctorById(Guid doctorId)
        {
            var doctor = await _doctorService.GetDoctorById(doctorId);
            return Success(doctor);
        
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult>UpdateDoctor(Guid id,UpdateDoctorDto dto)
        {
            var result = await _doctorService.UpdateDoctor(id, dto);

            return Success(result, "Doctor actualizado con exito");
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult>DeleteDoctor(Guid id)
        {
            var result = await _doctorService.DeleteDoctor(id);

            return Success(result, "Doctor eliminado");
        }
    }
}
