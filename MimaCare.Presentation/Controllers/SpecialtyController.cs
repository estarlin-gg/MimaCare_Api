using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MimaCare.Application.Dtos.Specialty;
using MimaCare.Application.Interface;
using MimaCare.Presentation.Common;

namespace MimaCare.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Clinic,Super")]
    public class SpecialtyController : BaseController
    {
        private readonly ISpecialtyService _specialtyService;

        public SpecialtyController(ISpecialtyService specialtyService)
        {
            _specialtyService = specialtyService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSpecialties()
        {
            var specialties = await _specialtyService.GetAllSpecialties();
            return Success(specialties);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSpecialty(CreateSpecialtyDto dto)
        {
            var specialty = await _specialtyService.CreateSpecialty(dto);
            return CreatedResponse(specialty, "Specialty created successfully.");
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetSpecialtyById(Guid id)
        {
            var specialty = await _specialtyService.GetSpecialtyById(id);
            return Success(specialty);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateSpecialty(Guid id, UpdateSpecialtyDto dto)
        {
            var result = await _specialtyService.UpdateSpecialty(id, dto);
            return Success(result, "Specialty updated successfully.");
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteSpecialty(Guid id)
        {
            var result = await _specialtyService.DeleteSpecialty(id);
            return Success(result, "Specialty deleted.");
        }
    }
}
