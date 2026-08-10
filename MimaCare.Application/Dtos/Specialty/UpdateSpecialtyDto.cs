using System;
using System.Collections.Generic;
using System.Text;

namespace MimaCare.Application.Dtos.Specialty
{
    public class UpdateSpecialtyDto
    {
        public string Name { get; set; }
            = default!;

        public string? Description { get; set; }

        public bool IsActive { get; set; }
    }
}
