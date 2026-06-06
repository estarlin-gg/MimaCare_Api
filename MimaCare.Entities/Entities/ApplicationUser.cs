using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace MimaCare.Domain.Entities
{
    public class ApplicationUser: IdentityUser<Guid>
    {
        public string FirstName { get; set; } = default!;

        public string LastName { get; set; } = default!;

        public Guid ClinicId { get; set; }

        public Clinic Clinic { get; set; } = default!;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; }
            = DateTime.UtcNow;

        // Navigation
        public Doctor? Doctor { get; set; }

        public ICollection<PatientFile> UploadedFiles { get; set; }
            = new List<PatientFile>();


    }
}
