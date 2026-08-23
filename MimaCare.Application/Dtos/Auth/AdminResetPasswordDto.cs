using System;
using System.Collections.Generic;
using System.Text;

namespace MimaCare.Application.Dtos.Auth
{
    public class AdminResetPasswordDto
    {
        public string NewPassword { get; set; }
            = default!;
    }
}
