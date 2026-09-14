namespace MimaCare.Application.Dtos.SubscriptionPlan
{
    public class UpdateSubscriptionPlanDto
    {
        public string Name { get; set; } = default!;

        public string? Description { get; set; }

        public int? MaxBranches { get; set; }

        public int? MaxDoctors { get; set; }

        public bool AllowsUnlimitedAppointments { get; set; } = true;

        public bool IsActive { get; set; } = true;
    }
}