using System;
using System.Collections.Generic;
using System.Text;

namespace MimaCare.Application.Dtos.Doctor
{
    public class UpdateDoctorDto
    {
        public string FirstName { get; set; }
       = default!;

        public string LastName { get; set; }
            = default!;

        public string Email { get; set; }
            = default!;

        public string PhoneNumber { get; set; }
            = default!;

        public string Specialty { get; set; }
            = default!;

        public string? LicenseNumber { get; set; }

        public bool IsAvailable { get; set; }

        
    }
}
