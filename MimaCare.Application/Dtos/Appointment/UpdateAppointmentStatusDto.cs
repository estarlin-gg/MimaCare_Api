using MimaCare.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace MimaCare.Application.Dtos.Appointment
{
    public class UpdateAppointmentStatusDto
    {
        public AppointmentStatus Status { get; set; }
    }
}
