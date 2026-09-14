using System;

namespace MimaCare.Application.Dtos.SubscriptionPlan
{
    public class SubscriptionPlanDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = default!;

        public string? Description { get; set; }

        public int? MaxBranches { get; set; }

        public int? MaxDoctors { get; set; }

        public bool AllowsUnlimitedAppointments { get; set; }

        public bool IsActive { get; set; }
    }
}