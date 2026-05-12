using FluentValidation;
using MimaCare.Application.Dtos.Auth;
using System;
using System.Collections.Generic;
using System.Text;

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

            RuleFor(x => x.ClinicEmail)
                .NotEmpty()
                .WithMessage("Clinic email is required.")
                .EmailAddress()
                .WithMessage("Invalid clinic email.");

            RuleFor(x => x.ClinicPhone)
                .NotEmpty()
                .WithMessage("Clinic phone is required.")
                .MaximumLength(30);

            RuleFor(x => x.ClinicAddress)
                .NotEmpty()
                .WithMessage("Clinic address is required.")
                .MaximumLength(300);

            

            RuleFor(x => x.FirstName)
                .NotEmpty()
                .WithMessage("First name is required.")
                .MaximumLength(100);

            RuleFor(x => x.LastName)
                .NotEmpty()
                .WithMessage("Last name is required.")
                .MaximumLength(100);

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email is required.")
                .EmailAddress()
                .WithMessage("Invalid email.");

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Password is required.")
                .MinimumLength(8)
                .WithMessage("Password must be at least 8 characters.")
                .Matches("[A-Z]")
                .WithMessage("Password must contain at least one uppercase letter.")
                .Matches("[a-z]")
                .WithMessage("Password must contain at least one lowercase letter.")
                .Matches("[0-9]")
                .WithMessage("Password must contain at least one number.");
        }
    }
}
