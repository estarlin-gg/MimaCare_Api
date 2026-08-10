namespace MimaCare.Domain.Entities
{
    public class Patient
    {

        public Guid Id { get; set; }

        public Guid ClinicId { get; set; }

        public Clinic Clinic { get; set; }
            = default!;

        public string FirstName { get; set; }
            = default!;

        public string LastName { get; set; }
            = default!;

        public string Phone { get; set; }
            = default!;

        public string Address { get; set; }
            = default!;

        public string? Allergies { get; set; }

        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; }
            = DateTime.UtcNow;

        // Navigation
        public ICollection<Appointment> Appointments { get; set; }
            = new List<Appointment>();

        public ICollection<PatientFile> Files { get; set; }
            = new List<PatientFile>();
    }
}
