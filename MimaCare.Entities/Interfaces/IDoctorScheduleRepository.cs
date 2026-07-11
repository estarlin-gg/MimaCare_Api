using MimaCare.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MimaCare.Domain.Interfaces
{
    public interface IDoctorScheduleRepository
    {
        Task CreateSchedule(DoctorSchedule schedule);
        Task UpdateSchedule(DoctorSchedule schedule);
        Task DeleteSchedule(DoctorSchedule schedule);
        Task<DoctorSchedule?> GetScheduleById(Guid id);
        Task<IEnumerable<DoctorSchedule>> GetSchedulesByDoctorId(Guid doctorId);
        Task<DoctorSchedule?> GetScheduleByDoctorAndDay(Guid doctorId, DayOfWeek dayOfWeek);
        Task SaveChangesAsync();
    }
}
