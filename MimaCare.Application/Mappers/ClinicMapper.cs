using Mapster;
using MimaCare.Application.Dtos.Auth;
using MimaCare.Domain.Entities;

namespace MimaCare.Application.Mappers
{
    public class ClinicMapper : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<RegisterClinicDto, Clinic>()
                .Map(dest => dest.Name, src => src.ClinicName)
                .Map(dest => dest.Email, src => src.Email)
                .Map(dest => dest.Phone, src => src.ClinicPhone)
                .Map(dest => dest.Address, src => src.ClinicAddress);
        }
    }
}
