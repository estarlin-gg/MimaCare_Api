using System;
using System.Collections.Generic;
using System.Text;

namespace MimaCare.Domain.Enums
{
    public enum AppointmentStatus
    {
        Pending = 1,
        Confirmed = 2,
        Completed = 3,
        Cancelled = 4,
        Rejected = 5
    }
}
