using MimaCare.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace MimaCare.Domain.Entities
{
    public class Appointment
    {
        public Guid Id { get; set; }

        public Guid ClinicId { get; set; }

        public Clinic Clinic { get; set; }


        public Guid PatientId { get; set; }

        public Patient Patient { get; set; }
            

        public Guid DoctorId { get; set; }

        public Doctor Doctor { get; set; }
            
        public DateOnly AppointmentDate { get; set; }

        public TimeOnly? StartTime { get; set; }

        public TimeOnly? EndTime { get; set; }

        public AppointmentStatus Status { get; set; }

        public string? Notes { get; set; }

        // Branch where the patient is (or was) attended.
        public Guid? BranchId { get; set; }

        public Branch? Branch { get; set; }

        // Branch where the appointment was created.
        public Guid? CreatedBranchId { get; set; }

        public Branch? CreatedBranch { get; set; }

        public DateTime CreatedAt { get; set; }
            = DateTime.UtcNow;
    }
}
