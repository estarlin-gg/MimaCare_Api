using System;
using System.Collections.Generic;
using System.Text;

namespace MimaCare.Domain.Entities
{
    public class DoctorSchedule
    {
        public Guid Id { get; set; }

        public Guid DoctorId { get; set; }
        public Doctor Doctor { get; set; } = default!;

        public DayOfWeek DayOfWeek { get; set; }

        public TimeOnly StartTime { get; set; }

        public TimeOnly EndTime { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
