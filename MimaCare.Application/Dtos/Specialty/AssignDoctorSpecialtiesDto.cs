using System;
using System.Collections.Generic;
using System.Text;

namespace MimaCare.Application.Dtos.Specialty
{
    public class AssignDoctorSpecialtiesDto
    {
        public List<Guid> SpecialtyIds { get; set; }
            = new List<Guid>();
    }
}
