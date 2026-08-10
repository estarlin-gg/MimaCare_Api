using MimaCare.Application.Dtos.Doctor;
using System;
using System.Collections.Generic;
using System.Text;

namespace MimaCare.Application.Interface
{
    public interface IDoctorService
    {

        Task<string>  CreateDoctor(CreateDoctorDto dto);

        Task<DoctorDto> GetDoctorById(Guid id);

        Task<IEnumerable<DoctorDto>> GetAllDoctor();

        Task<string> UpdateDoctor(Guid doctorId, UpdateDoctorDto dto);
        Task<string> DeleteDoctor(Guid doctorId);

        Task<string> ResetDoctorPassword(Guid doctorId);

        Task<IEnumerable<string>> GetSpecialties();

    }
}
