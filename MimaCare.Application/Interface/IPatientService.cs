using MimaCare.Application.Dtos.Patient;

namespace MimaCare.Application.Interface
{
    public interface IPatientService
    {
        Task<IEnumerable<PatientDto>> GetPatients();
        Task<PatientDto> GetPatientById(Guid id);
        Task<string> CreatePatient(CreatePatientDto dto);
        Task<string> UpdatePatient(Guid id, UpdatePatientDto dto);
        Task<string> DeletePatient(Guid id);
    }
}
