using System;
using System.Collections.Generic;

namespace MimaCare.Domain.Entities
{
    public class SubscriptionPlan
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = default!;

        public string? Description { get; set; }

        // null = unbounded
        public int? MaxBranches { get; set; }

        // null = unbounded
        public int? MaxDoctors { get; set; }

        public bool AllowsUnlimitedAppointments { get; set; } = true;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; }
            = DateTime.UtcNow;

        public ICollection<Clinic> Clinics { get; set; }
            = new List<Clinic>();
    }
}