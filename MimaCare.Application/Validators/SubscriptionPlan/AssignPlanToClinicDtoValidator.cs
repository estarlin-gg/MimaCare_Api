using FluentValidation;
using MimaCare.Application.Dtos.SubscriptionPlan;

namespace MimaCare.Application.Validators.SubscriptionPlan
{
    public class AssignPlanToClinicDtoValidator : AbstractValidator<AssignPlanToClinicDto>
    {
        public AssignPlanToClinicDtoValidator()
        {
            RuleFor(x => x.PlanId)
                .NotEmpty().WithMessage("Plan is required.");
        }
    }
}