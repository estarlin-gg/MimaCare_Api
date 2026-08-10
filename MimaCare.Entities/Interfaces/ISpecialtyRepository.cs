using MimaCare.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MimaCare.Domain.Interfaces
{
    public interface ISpecialtyRepository
    {
        Task CreateSpecialty(Specialty specialty);
        Task UpdateSpecialty(Specialty specialty);
        Task DeleteSpecialty(Specialty specialty);
        Task<Specialty?> GetSpecialtyById(Guid id);
        Task<IEnumerable<Specialty>> GetAllSpecialties();
        Task<IEnumerable<Specialty>> GetSpecialtiesByDoctorId(Guid doctorId);
        Task<IEnumerable<DoctorSpecialty>> GetDoctorSpecialtyRelations(Guid doctorId);
        Task AddDoctorSpecialty(DoctorSpecialty relation);
        Task DeleteDoctorSpecialty(DoctorSpecialty relation);
        Task SaveChangesAsync();
    }
}
