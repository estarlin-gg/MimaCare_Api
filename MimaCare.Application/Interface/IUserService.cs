using MimaCare.Application.Dtos.Auth;
using MimaCare.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MimaCare.Application.Interface
{
    public interface IUserService
    {
        Task<ApplicationUser> CreateUserAsync(CreateUserDto userDto);
    }
}
