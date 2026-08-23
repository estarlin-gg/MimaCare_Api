using System;
using System.Collections.Generic;
using System.Text;

namespace MimaCare.Application.Dtos.Auth
{
    public class ResetPasswordDto
    {
        public string Email { get; set; }
            = default!;

        public string Token { get; set; }
            = default!;

        public string NewPassword { get; set; }
            = default!;
    }
}
