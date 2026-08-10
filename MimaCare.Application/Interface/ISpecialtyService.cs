using MimaCare.Application.Dtos.Specialty;
using System;
using System.Collections.Generic;
using System.Text;

namespace MimaCare.Application.Interface
{
    public interface ISpecialtyService
    {
        Task<SpecialtyDto> CreateSpecialty(CreateSpecialtyDto dto);

        Task<IEnumerable<SpecialtyDto>> GetAllSpecialties();

        Task<SpecialtyDto> GetSpecialtyById(Guid id);

        Task<string> UpdateSpecialty(Guid id, UpdateSpecialtyDto dto);

        Task<string> DeleteSpecialty(Guid id);

        Task<IEnumerable<SpecialtyDto>> GetDoctorSpecialties(Guid doctorId);

        Task<string> AssignDoctorSpecialties(Guid doctorId, AssignDoctorSpecialtiesDto dto);
    }
}
