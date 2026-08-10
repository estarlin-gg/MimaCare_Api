using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace MimaCare.Domain.Entities
{
    public class Clinic
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = default!;

        public string Email { get; set; } = default!;

        public string? Phone { get; set; }

        public string? Address { get; set; }

        public string? LogoUrl { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; }
            = DateTime.UtcNow;


        public ICollection<ApplicationUser> Users { get; set; }
            = new List<ApplicationUser>();

        public ICollection<Doctor> Doctors { get; set; }
            = new List<Doctor>();

        public ICollection<Patient> Patients { get; set; }
            = new List<Patient>();

        public ICollection<Appointment> Appointments { get; set; }
            = new List<Appointment>();

    }
}
