using System;
using System.Collections.Generic;
using System.Text;

namespace MimaCare.Application.Dtos.Auth
{
    public class CreateUserDto
    {
        public string FirstName { get; set; }
            = default!;

        public string LastName { get; set; }
            = default!;

        public string Email { get; set; }
            = default!;

        public string Password { get; set; }
            = default!;

        public string? PhoneNumber { get; set; }

        public Guid ClinicId { get; set; }

        public string Role { get; set; }
            = default!;
    }
}
