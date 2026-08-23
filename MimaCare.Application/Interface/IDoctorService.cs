using MimaCare.Application.Dtos.Common;
using MimaCare.Application.Dtos.Doctor;
using MimaCare.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace MimaCare.Application.Interface
{
    public interface IDoctorService
    {

        Task<string>  CreateDoctor(CreateDoctorDto dto);

        Task<DoctorDto> GetDoctorById(Guid id);

        Task<PagedResult<DoctorDto>> GetAllDoctor(PaginationParams pagination);

        Task<string> UpdateDoctor(Guid doctorId, UpdateDoctorDto dto);
        Task<string> DeleteDoctor(Guid doctorId);

        Task<string> ResetDoctorPassword(Guid doctorId);

        Task<IEnumerable<string>> GetSpecialties();

    }
}
