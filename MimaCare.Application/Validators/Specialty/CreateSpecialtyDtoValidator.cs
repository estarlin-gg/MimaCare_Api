using FluentValidation;
using MimaCare.Application.Dtos.Specialty;

namespace MimaCare.Application.Validators.Specialty
{
    public class CreateSpecialtyDtoValidator : AbstractValidator<CreateSpecialtyDto>
    {
        public CreateSpecialtyDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description must not exceed 500 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.Description));
        }
    }
}
