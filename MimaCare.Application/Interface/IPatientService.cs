using MimaCare.Application.Dtos.Common;
using MimaCare.Application.Dtos.Patient;
using MimaCare.Domain.Common;

namespace MimaCare.Application.Interface
{
    public interface IPatientService
    {
        Task<PagedResult<PatientDto>> GetPatients(PaginationParams pagination);
        Task<PatientDto> GetPatientById(Guid id);
        Task<string> CreatePatient(CreatePatientDto dto);
        Task<string> UpdatePatient(Guid id, UpdatePatientDto dto);
        Task<string> DeletePatient(Guid id);
    }
}
