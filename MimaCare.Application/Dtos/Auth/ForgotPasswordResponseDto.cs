using System;
using System.Collections.Generic;
using System.Text;

namespace MimaCare.Application.Dtos.Auth
{
    public class ForgotPasswordResponseDto
    {
        public string Message { get; set; }
            = default!;

        /// <summary>
        /// Only populated in Development (no email provider configured).
        /// </summary>
        public string? ResetToken { get; set; }
    }
}
