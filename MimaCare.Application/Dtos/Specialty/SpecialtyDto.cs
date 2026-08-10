using System;
using System.Collections.Generic;
using System.Text;

namespace MimaCare.Application.Dtos.Specialty
{
    public class SpecialtyDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
            = default!;

        public string? Description { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
