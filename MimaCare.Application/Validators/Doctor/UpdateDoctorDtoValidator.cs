using FluentValidation;
using MimaCare.Application.Dtos.Doctor;
using System;
using System.Collections.Generic;
using System.Text;

namespace MimaCare.Application.Validators.Doctor
{
    public class UpdateDoctorDtoValidator: AbstractValidator<UpdateDoctorDto>
    {
        public UpdateDoctorDtoValidator()
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

            //RuleFor(x => x.Password)
            //    .NotEmpty()
            //    .WithMessage("Password is required.")
            //    .MinimumLength(8)
            //    .WithMessage(
            //        "Password must be at least 8 characters.");

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
