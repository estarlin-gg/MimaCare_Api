using System;
using System.Collections.Generic;
using System.Text;

namespace MimaCare.Application.Dtos.Patient
{
    public class CreatePatientDto
    {
        public string FirstName { get; set; }
       = default!;

        public string LastName { get; set; }
            = default!;

        public string Phone { get; set; }
            = default!;

        public string Address { get; set; }
            = default!;

        public string? Allergies { get; set; }
    }
}
