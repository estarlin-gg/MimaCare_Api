using System;
using System.Collections.Generic;
using System.Text;

namespace MimaCare.Application.Dtos.Doctor
{
    public class CreateDoctorDto
    {
        public string FirstName { get; set; }
      = default!;

        public string LastName { get; set; }
            = default!;

        public string Email { get; set; }
            = default!;

        public string? Password { get; set; }

        public string PhoneNumber { get; set; }
            = default!;

        public string Specialty { get; set; }
            = default!;

        public string? LicenseNumber { get; set; }

        // Branches/sucursales where the doctor will work.
        // Optional: the doctor can be assigned to branches later.
        public List<Guid>? BranchIds { get; set; }
    }
}
