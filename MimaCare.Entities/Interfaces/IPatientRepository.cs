using MimaCare.Domain.Common;
using MimaCare.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MimaCare.Domain.Interfaces
{
    public interface IPatientRepository
    {
        Task CreatePatient(
          Patient patient);

        Task UpdatePatient(
            Patient patient);

        Task DeletePatient(
            Patient patient);

        Task<Patient?> GetPatientById(
            Guid patientId,
            Guid clinicId);

        Task<PagedResult<Patient>> GetPatients(
            Guid clinicId,
            int page,
            int pageSize,
            string? search = null);
    }
}
