using System;
using System.Collections.Generic;
using System.Text;

namespace MimaCare.Application.Dtos.Appointment
{
    public class CreateAppointmentDto
    {
        public Guid DoctorId { get; set; }

        public Guid PatientId { get; set; }

        public DateOnly AppointmentDate { get; set; }

        public TimeOnly StartTime { get; set; }

        public TimeOnly EndTime { get; set; }

        public string? Notes { get; set; }

        // Branch where the patient will be attended.
        public Guid BranchId { get; set; }

        // Optional: branch where the appointment was created.
        // Defaults to BranchId when not provided.
        public Guid? CreatedBranchId { get; set; }
    }
}
