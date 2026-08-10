using System;
using System.Collections.Generic;
using System.Text;

namespace MimaCare.Application.Dtos.Patient
{
    public class PatientDto
    {
        public Guid Id { get; set; }

        public string FullName { get; set; }
            = default!;

        public string Phone { get; set; }
            = default!;

        public string Address { get; set; }
            = default!;

        public string? Allergies { get; set; }

        public string? Notes { get; set; }

        public string ClinicName { get; set; }
            = default!;

        public DateTime CreatedAt { get; set; }
    }
}
