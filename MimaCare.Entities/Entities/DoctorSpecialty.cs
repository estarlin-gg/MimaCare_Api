using System;
using System.Collections.Generic;
using System.Text;

namespace MimaCare.Domain.Entities
{
    public class DoctorSpecialty
    {
        public Guid DoctorId { get; set; }

        public Doctor Doctor { get; set; }
            = default!;

        public Guid SpecialtyId { get; set; }

        public Specialty Specialty { get; set; }
            = default!;
    }
}
