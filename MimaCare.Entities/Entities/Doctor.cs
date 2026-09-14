using System;
using System.Collections.Generic;
using System.Text;

namespace MimaCare.Domain.Entities
{
    public class Doctor
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; } = default!;

        public ApplicationUser User { get; set; }
            = default!;

        public Guid ClinicId { get; set; }

        public Clinic Clinic { get; set; }
            = default!;

        public string Specialty { get; set; }
            = default!;

        public string? LicenseNumber { get; set; }

        public string? ProfileImageUrl { get; set; }

        public bool IsAvailable { get; set; } = true;

        public DateTime CreatedAt { get; set; }
            = DateTime.UtcNow;

        // Navigation
        public ICollection<Appointment> Appointments { get; set; }
            = new List<Appointment>();

        public ICollection<DoctorSchedule> Schedules { get; set; }
            = new List<DoctorSchedule>();

        public ICollection<DoctorSpecialty> DoctorSpecialties { get; set; }
            = new List<DoctorSpecialty>();

        public ICollection<DoctorBranch> DoctorBranches { get; set; }
            = new List<DoctorBranch>();
    }
}
