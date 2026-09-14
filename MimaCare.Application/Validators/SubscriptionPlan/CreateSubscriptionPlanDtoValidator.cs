using FluentValidation;
using MimaCare.Application.Dtos.SubscriptionPlan;

namespace MimaCare.Application.Validators.SubscriptionPlan
{
    public class CreateSubscriptionPlanDtoValidator : AbstractValidator<CreateSubscriptionPlanDto>
    {
        public CreateSubscriptionPlanDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(50);

            RuleFor(x => x.Description)
                .MaximumLength(500)
                .When(x => !string.IsNullOrWhiteSpace(x.Description));

            RuleFor(x => x.MaxBranches)
                .GreaterThan(0).WithMessage("MaxBranches must be positive.")
                .When(x => x.MaxBranches.HasValue);

            RuleFor(x => x.MaxDoctors)
                .GreaterThan(0).WithMessage("MaxDoctors must be positive.")
                .When(x => x.MaxDoctors.HasValue);
        }
    }
}