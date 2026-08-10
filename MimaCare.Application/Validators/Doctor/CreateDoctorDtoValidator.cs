using FluentValidation;
using MimaCare.Application.Dtos.Doctor;
using System;
using System.Collections.Generic;
using System.Text;

namespace MimaCare.Application.Validators.Doctor
{
    public class CreateDoctorDtoValidator: AbstractValidator<CreateDoctorDto>
    {
        public CreateDoctorDtoValidator()
        {
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
                .WithMessage("Invalid email format.");

            RuleFor(x => x.Password)
                .MinimumLength(6)
                .WithMessage(
                    "Password must be at least 6 characters.")
                .When(x => !string.IsNullOrWhiteSpace(
                    x.Password));

            RuleFor(x => x.PhoneNumber)
                .NotEmpty()
                .WithMessage("Phone number is required.");

            RuleFor(x => x.Specialty)
                .NotEmpty()
                .WithMessage("Specialty is required.")
                .MaximumLength(100);

            RuleFor(x => x.LicenseNumber)
                .MaximumLength(100)
                .When(x => !string.IsNullOrWhiteSpace(
                    x.LicenseNumber));

        }
    }
}
