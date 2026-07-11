using FluentValidation;
using MimaCare.Application.Dtos.DoctorSchedule;

namespace MimaCare.Application.Validators.DoctorSchedule
{
    public class CreateDoctorScheduleDtoValidator : AbstractValidator<CreateDoctorScheduleDto>
    {
        public CreateDoctorScheduleDtoValidator()
        {
            RuleFor(x => x.DayOfWeek)
                .IsInEnum().WithMessage("Invalid day of week.");

            RuleFor(x => x.StartTime)
                .NotEmpty().WithMessage("Start time is required.");

            RuleFor(x => x.EndTime)
                .NotEmpty().WithMessage("End time is required.")
                .GreaterThan(x => x.StartTime)
                .WithMessage("End time must be after start time.");
        }
    }
}
