using System;
using System.Collections.Generic;
using System.Text;

namespace MimaCare.Application.Dtos.DoctorSchedule
{
    public class CreateDoctorScheduleDto
    {
        public DayOfWeek DayOfWeek { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
