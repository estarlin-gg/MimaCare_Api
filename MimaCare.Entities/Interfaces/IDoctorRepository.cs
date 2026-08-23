using MimaCare.Domain.Common;
using MimaCare.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MimaCare.Infrastructure.Repositories.Abstraction
{
    public interface IDoctorRepository
    {
        Task CreateDoctor(Doctor doctor);

        Task<Doctor?> GetDoctorById(Guid doctorId);

        Task<PagedResult<Doctor>> GetDoctors(Guid clinicId, int page, int pageSize);

        Task UpdateDoctor(Doctor doctor);

        Task DeleteDoctor(Doctor doctor);
    }
}
