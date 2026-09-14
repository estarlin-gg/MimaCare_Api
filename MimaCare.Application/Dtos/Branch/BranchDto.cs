using System;

namespace MimaCare.Application.Dtos.Branch
{
    public class BranchDto
    {
        public Guid Id { get; set; }

        public Guid ClinicId { get; set; }

        public string Name { get; set; } = default!;

        public string? Address { get; set; }

        public string? Phone { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}