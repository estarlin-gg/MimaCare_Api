using System;

namespace MimaCare.Application.Dtos.Appointment
{
    public class UpdateAppointmentDto
    {
        public Guid PatientId { get; set; }

        public DateOnly AppointmentDate { get; set; }

        public TimeOnly StartTime { get; set; }

        public TimeOnly EndTime { get; set; }

        public string? Notes { get; set; }
    }
}
