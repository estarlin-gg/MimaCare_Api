using Microsoft.AspNetCore.Identity;
using MimaCare.Application.Common.Exceptions;
using MimaCare.Application.Dtos.Auth;
using MimaCare.Application.Interface;
using MimaCare.Domain.Entities;

namespace MimaCare.Application.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser>
       _userManager;

        public UserService(
            UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<ApplicationUser>
            CreateUserAsync(CreateUserDto userDto)
        {
            var existingUser =
                await _userManager
                    .FindByEmailAsync(userDto.Email);

            if (existingUser != null)
            {
                throw new ConflictException(
                    "Email already exists.");
            }

            var user = new ApplicationUser
            {
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                Email = userDto.Email,
                UserName = userDto.Email,
                PhoneNumber = userDto.PhoneNumber,
                ClinicId = userDto.ClinicId,
                IsActive = true
            };

            var result =
                await _userManager
                    .CreateAsync(
                        user,
                        userDto.Password);

            if (!result.Succeeded)
            {
                throw new BadRequestException(
                    string.Join(
                        ", ",
                        result.Errors
                            .Select(x =>
                                x.Description)));
            }

            var roleResult =
                await _userManager
                    .AddToRoleAsync(
                        user,
                        userDto.Role);

            if (!roleResult.Succeeded)
            {
                throw new BadRequestException(
                    string.Join(
                        ", ",
                        roleResult.Errors
                            .Select(x =>
                                x.Description)));
            }

            return user;
        }
    }
}
