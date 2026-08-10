using System;
using System.Collections.Generic;
using System.Text;

namespace MimaCare.Application.Dtos.Specialty
{
    public class CreateSpecialtyDto
    {
        public string Name { get; set; }
            = default!;

        public string? Description { get; set; }
    }
}
