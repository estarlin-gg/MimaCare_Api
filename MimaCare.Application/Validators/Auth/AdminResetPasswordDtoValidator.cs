using FluentValidation;
using MimaCare.Application.Dtos.Auth;

namespace MimaCare.Application.Validators.Auth
{
    public class AdminResetPasswordDtoValidator
     : AbstractValidator<AdminResetPasswordDto>
    {
        public AdminResetPasswordDtoValidator()
        {
            RuleFor(x => x.NewPassword)
                .NotEmpty()
                .WithMessage("New password is required.")
                .MinimumLength(6)
                .WithMessage("Password must be at least 6 characters.");
        }
    }
}
