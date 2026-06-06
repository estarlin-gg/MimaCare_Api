using MimaCare.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MimaCare.Application.Dtos.Doctor
{
    public class DoctorDto
    {
        public Guid Id { get; set; }

        public string FullName { get; set; }
        public string ClinicName { get; set; }

        public string Specialty { get; set; }
            = default!;

        public string? LicenseNumber { get; set; }

        public string? ProfileImageUrl { get; set; }

        public bool IsAvailable { get; set; }


        public string Email { get;  set; }

        public string PhoneNumber { get; set; }
    }
}
