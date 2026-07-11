using System;
using System.Collections.Generic;
using System.Text;

namespace MimaCare.Application.Dtos.DoctorSchedule
{
    public class DoctorScheduleDto
    {
        public Guid Id { get; set; }
        public Guid DoctorId { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public bool IsActive { get; set; }
    }
}
