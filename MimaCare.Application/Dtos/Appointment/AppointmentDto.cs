using MimaCare.Domain.Entities;
using MimaCare.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace MimaCare.Application.Dtos.Appointment
{
    public class AppointmentDto
    {
        public Guid Id { get; set; }

        public Guid ClinicId { get; set; }

        public Guid DoctorId { get; set; }

        public Guid PatientId { get; set; }

        public string ClinicName { get; set; }
            = default!;

        public string PatientName { get; set; }
            = default!;

        public string DoctorName { get; set; }
            = default!;

        public Guid? BranchId { get; set; }

        public string? BranchName { get; set; }

        public Guid? CreatedBranchId { get; set; }

        public string? CreatedBranchName { get; set; }

        public DateOnly AppointmentDate { get; set; }

        public TimeOnly? StartTime { get; set; }

        public TimeOnly? EndTime { get; set; }

        public AppointmentStatus Status { get; set; }

        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; }

    }
}

