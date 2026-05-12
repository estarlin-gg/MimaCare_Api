using FluentValidation;
using MimaCare.Application.Dtos.Auth;

namespace MimaCare.Application.Validators.Auth
{
    public class LoginDtoValidator
     : AbstractValidator<LoginDto>
    {
        public LoginDtoValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email is required.")
                .EmailAddress()
                .WithMessage("Invalid email.");

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Password is required.");
        }
    }
}
