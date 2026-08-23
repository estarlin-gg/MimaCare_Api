using System;
using System.Collections.Generic;
using System.Text;

namespace MimaCare.Application.Dtos.Auth
{
    public class ForgotPasswordDto
    {
        public string Email { get; set; }
            = default!;
    }
}
