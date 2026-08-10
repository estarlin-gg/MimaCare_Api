using FluentValidation;
using MimaCare.Application.Dtos.Patient;

namespace MimaCare.Application.Validators.Patient
{
    public class UpdatePatientDtoValidator : AbstractValidator<UpdatePatientDto>
    {
        public UpdatePatientDtoValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required.")
                .MaximumLength(100);

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required.")
                .MaximumLength(100);

            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage("Phone is required.")
                .MaximumLength(30);

            RuleFor(x => x.Address)
                .NotEmpty().WithMessage("Address is required.")
                .MaximumLength(300);

            RuleFor(x => x.Allergies)
                .MaximumLength(500);

            RuleFor(x => x.Notes)
                .MaximumLength(2000);
        }
    }
}
