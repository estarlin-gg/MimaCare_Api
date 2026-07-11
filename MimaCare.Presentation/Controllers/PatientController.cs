using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MimaCare.Application.Dtos.Patient;
using MimaCare.Application.Interface;
using MimaCare.Presentation.Common;

namespace MimaCare.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PatientController : BaseController
    {
        private readonly IPatientService _patientService;

        public PatientController(IPatientService patientService)
        {
            _patientService = patientService;
        }

        [HttpGet]
        public async Task<IActionResult> GetPatients()
        {
            var patients = await _patientService.GetPatients();
            return Success(patients);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetPatientById(Guid id)
        {
            var patient = await _patientService.GetPatientById(id);
            return Success(patient);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePatient(CreatePatientDto dto)
        {
            var result = await _patientService.CreatePatient(dto);
            return CreatedResponse(result);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdatePatient(Guid id, UpdatePatientDto dto)
        {
            var result = await _patientService.UpdatePatient(id, dto);
            return Success(result);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeletePatient(Guid id)
        {
            var result = await _patientService.DeletePatient(id);
            return Success(result, "Patient deleted successfully.");
        }
    }
}
