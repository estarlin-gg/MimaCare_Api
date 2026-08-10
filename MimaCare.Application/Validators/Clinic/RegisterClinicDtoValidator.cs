using FluentValidation;
using MimaCare.Application.Dtos.Auth;

namespace MimaCare.Application.Validators.Clinic
{
    public class RegisterClinicDtoValidator
     : AbstractValidator<RegisterClinicDto>
    {
        public RegisterClinicDtoValidator()
        {
            RuleFor(x => x.ClinicName)
                .NotEmpty()
                .WithMessage("Clinic name is required.")
                .MaximumLength(150);

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email is required.")
                .EmailAddress()
                .WithMessage("Invalid email.");

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Password is required.")
                .MinimumLength(6)
                .WithMessage("Password must be at least 6 characters.");
        }
    }
}
