using MimaCare.Application.Dtos.DoctorSchedule;

namespace MimaCare.Application.Interface
{
    public interface IDoctorScheduleService
    {
        Task<IEnumerable<DoctorScheduleDto>> GetSchedulesByDoctor(Guid doctorId);
        Task SetDoctorSchedule(Guid doctorId, IEnumerable<CreateDoctorScheduleDto> schedules);
        Task ClearDoctorSchedule(Guid doctorId);
    }
}
