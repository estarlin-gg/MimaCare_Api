using Mapster;
using MimaCare.Application.Dtos.DoctorSchedule;
using MimaCare.Domain.Entities;

namespace MimaCare.Application.Mappers
{
    public class DoctorScheduleMapper : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<DoctorSchedule, DoctorScheduleDto>();

            config.NewConfig<CreateDoctorScheduleDto, DoctorSchedule>();

            config.NewConfig<UpdateDoctorScheduleDto, DoctorSchedule>();
        }
    }
}
