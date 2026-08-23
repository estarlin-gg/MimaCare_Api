using FluentValidation;
using MimaCare.Application.Dtos.Auth;

namespace MimaCare.Application.Validators.Auth
{
    public class ResetPasswordDtoValidator
     : AbstractValidator<ResetPasswordDto>
    {
        public ResetPasswordDtoValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email is required.")
                .EmailAddress()
                .WithMessage("Invalid email.");

            RuleFor(x => x.Token)
                .NotEmpty()
                .WithMessage("Reset token is required.");

            RuleFor(x => x.NewPassword)
                .NotEmpty()
                .WithMessage("New password is required.")
                .MinimumLength(6)
                .WithMessage("Password must be at least 6 characters.");
        }
    }
}
