using Mapster;
using MimaCare.Application.Dtos.Specialty;
using MimaCare.Domain.Entities;

namespace MimaCare.Application.Mappers
{
    public class SpecialtyMapper : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Specialty, SpecialtyDto>();

            config.NewConfig<CreateSpecialtyDto, Specialty>();

            config.NewConfig<UpdateSpecialtyDto, Specialty>();
        }
    }
}
