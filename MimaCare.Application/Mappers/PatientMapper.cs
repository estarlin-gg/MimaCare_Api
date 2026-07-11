using Mapster;
using MimaCare.Application.Dtos.Patient;
using MimaCare.Domain.Entities;

namespace MimaCare.Application.Mappers
{
    public class PatientMapper : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Patient, PatientDto>()
                .Map(dest => dest.FullName, src => $"{src.FirstName} {src.LastName}")
                .Map(dest => dest.ClinicName, src => src.Clinic.Name);

            config.NewConfig<CreatePatientDto, Patient>();

            config.NewConfig<UpdatePatientDto, Patient>();
        }
    }
}
