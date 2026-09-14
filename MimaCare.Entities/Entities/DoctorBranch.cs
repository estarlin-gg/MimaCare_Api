using System;

namespace MimaCare.Domain.Entities
{
    public class DoctorBranch
    {
        public Guid DoctorId { get; set; }

        public Doctor Doctor { get; set; } = default!;

        public Guid BranchId { get; set; }

        public Branch Branch { get; set; } = default!;
    }
}