using Mapster;
using Microsoft.AspNetCore.Http;
using MimaCare.Application.Common;
using MimaCare.Application.Common.Exceptions;
using MimaCare.Application.Dtos.Common;
using MimaCare.Application.Dtos.Patient;
using MimaCare.Application.Interface;
using MimaCare.Domain.Common;
using MimaCare.Domain.Entities;
using MimaCare.Domain.Interfaces;

namespace MimaCare.Application.Services
{
    public class PatientService : IPatientService
    {
        private readonly IPatientRepository _patientRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUnitOfWork _unitOfWork;

        public PatientService(
            IPatientRepository patientRepository,
            IHttpContextAccessor httpContextAccessor,
            IUnitOfWork unitOfWork)
        {
            _patientRepository = patientRepository;
            _httpContextAccessor = httpContextAccessor;
            _unitOfWork = unitOfWork;
        }

        public async Task<PagedResult<PatientDto>> GetPatients(PaginationParams pagination)
        {
            var clinicId = GetClinicId();
            var patients = await _patientRepository.GetPatients(
                clinicId,
                pagination.EffectivePage,
                pagination.EffectivePageSize,
                pagination.EffectiveSearch);
            return patients.AdaptTo<Patient, PatientDto>();
        }

        public async Task<PatientDto> GetPatientById(Guid id)
        {
            var clinicId = GetClinicId();
            var patient = await _patientRepository.GetPatientById(id, clinicId);
            if (patient == null)
                throw new NotFoundException("Patient not found.");
            return patient.Adapt<PatientDto>();
        }

        public async Task<string> CreatePatient(CreatePatientDto dto)
        {
            var clinicId = GetClinicId();
            var patient = dto.Adapt<Patient>();
            patient.ClinicId = clinicId;

            await _patientRepository.CreatePatient(patient);
            await _unitOfWork.CommitAsync();
            return "Patient created successfully.";
        }

        public async Task<string> UpdatePatient(Guid id, UpdatePatientDto dto)
        {
            var clinicId = GetClinicId();
            var patient = await _patientRepository.GetPatientById(id, clinicId);
            if (patient == null)
                throw new NotFoundException("Patient not found.");

            dto.Adapt(patient);
            await _patientRepository.UpdatePatient(patient);
            await _unitOfWork.CommitAsync();
            return "Patient updated successfully.";
        }

        public async Task<string> DeletePatient(Guid id)
        {
            var clinicId = GetClinicId();
            var patient = await _patientRepository.GetPatientById(id, clinicId);
            if (patient == null)
                throw new NotFoundException("Patient not found.");

            await _patientRepository.DeletePatient(patient);
            await _unitOfWork.CommitAsync();
            return "Patient deleted successfully.";
        }

        private Guid GetClinicId()
        {
            var claim = _httpContextAccessor.HttpContext?.User?.FindFirst("clinicId")?.Value;
            if (string.IsNullOrEmpty(claim) || !Guid.TryParse(claim, out var clinicId))
                throw new UnauthorizedException("Clinic not found.");
            return clinicId;
        }
    }
}
