using Mapster;
using MimaCare.Application.Dtos.Auth;
using MimaCare.Domain.Entities;

namespace MimaCare.Application.Mappers
{
    public class UserMapper : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<RegisterClinicDto, ApplicationUser>();
        }
    }
}
