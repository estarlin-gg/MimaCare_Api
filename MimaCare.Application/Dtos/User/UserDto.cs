using System;
using System.Collections.Generic;
using System.Text;

namespace MimaCare.Application.Dtos.User
{
    public class UserDto
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public bool IsActive { get; set; } = true;

        public string Email { get; set; }
    }
}
