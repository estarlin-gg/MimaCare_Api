using System;
using System.Collections.Generic;

namespace MimaCare.Domain.Entities
{
    public class Branch
    {
        public Guid Id { get; set; }

        public Guid ClinicId { get; set; }

        public Clinic Clinic { get; set; } = default!;

        public string Name { get; set; } = default!;

        public string? Address { get; set; }

        public string? Phone { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; }
            = DateTime.UtcNow;

        // Navigation: doctors assigned to this branch (many-to-many)
        public ICollection<DoctorBranch> DoctorBranches { get; set; }
            = new List<DoctorBranch>();

        // Navigation: appointments attended in this branch
        public ICollection<Appointment> AttendedAppointments { get; set; }
            = new List<Appointment>();

        public ICollection<Appointment> CreatedAppointments { get; set; }
            = new List<Appointment>();
    }
}