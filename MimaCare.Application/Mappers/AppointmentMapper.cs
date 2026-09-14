using Mapster;
using MimaCare.Application.Dtos.Appointment;
using MimaCare.Domain.Entities;
using MimaCare.Domain.Enums;

namespace MimaCare.Application.Mappers
{
    public class AppointmentMapper : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Appointment, AppointmentDto>()
                .Map(dest => dest.DoctorId, src => src.Doctor.UserId)
                .Map(dest => dest.ClinicName, src => src.Clinic.Name)
                .Map(dest => dest.PatientName, src => $"{src.Patient.FirstName} {src.Patient.LastName}")
                .Map(dest => dest.DoctorName, src => $"{src.Doctor.User.FirstName} {src.Doctor.User.LastName}")
                .Map(dest => dest.BranchName, src => src.Branch == null ? null : src.Branch.Name)
                .Map(dest => dest.CreatedBranchName, src => src.CreatedBranch == null ? null : src.CreatedBranch.Name);

            config.NewConfig<CreateAppointmentDto, Appointment>()
                .Map(dest => dest.Status, src => AppointmentStatus.Pending)
                .Map(dest => dest.CreatedAt, src => DateTime.UtcNow);
        }
    }
}
