using MimaCare.Domain.Common;
using MimaCare.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MimaCare.Domain.Interfaces
{
    public interface IDoctorRepository
    {
        Task CreateDoctor(Doctor doctor);

        Task<Doctor?> GetDoctorById(Guid doctorId);

        Task<Doctor?> GetDoctorByUserId(Guid userId);

        Task<PagedResult<Doctor>> GetDoctors(Guid clinicId, int page, int pageSize, string? search = null);

        Task UpdateDoctor(Doctor doctor);

        Task DeleteDoctor(Doctor doctor);
    }
}
