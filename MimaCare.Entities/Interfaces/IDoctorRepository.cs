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

        Task<IEnumerable<Doctor>> GetDoctors(Guid clinicId);

        Task UpdateDoctor(Doctor doctor);

        Task DeleteDoctor(Doctor doctor);
    }
}
