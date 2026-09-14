namespace MimaCare.Application.Dtos.SubscriptionPlan
{
    public class PlanUsageDto
    {
        public SubscriptionPlanDto? Plan { get; set; }

        public int BranchCount { get; set; }

        public int DoctorCount { get; set; }

        public bool CanCreateBranch { get; set; }

        public bool CanCreateDoctor { get; set; }
    }
}