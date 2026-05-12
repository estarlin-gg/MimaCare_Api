using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MimaCare.Application.Dtos.Auth;
using MimaCare.Application.Services.Abstraction;
using MimaCare.Domain.Entities;
using MimaCare.Infrastructure.Constants;
using MimaCare.Infrastructure.Context;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MimaCare.Application.Services.Implementation
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        private readonly SignInManager<ApplicationUser> _signInManager;

        private readonly IConfiguration _configuration;

        private readonly MimaCareContext _context;

        public AuthService(
         UserManager<ApplicationUser> userManager,
         SignInManager<ApplicationUser> signInManager,
         RoleManager<IdentityRole> roleManager,
         MimaCareContext context,
         IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _context = context;
        }


        public async Task<AuthResponseDto> Register(
            RegisterClinicDto dto)
        {


            var emailExists = await _userManager
                .FindByEmailAsync(dto.Email);

            if (emailExists != null)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Email already exists."
                };
            }



            var clinic = dto.Adapt<Clinic>();

            await _context.Clinics.AddAsync(clinic);

            await _context.SaveChangesAsync();



            var user = dto.Adapt<ApplicationUser>();

            user.UserName = dto.Email;

            user.Email = dto.Email;

            user.ClinicId = clinic.Id;

            var result = await _userManager
                .CreateAsync(user, dto.Password);

            if (!result.Succeeded)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = string.Join(
                        ", ",
                        result.Errors.Select(x => x.Description))
                };
            }



            await _userManager.AddToRoleAsync(
                user,
                Roles.Clinic
            );



            //var token = await GenerateToken(user);

            return new AuthResponseDto
            {
                Success = true,
                Message = "Clinic registered successfully.",
                //Token = token
            };
        }



        public async Task<AuthResponseDto> Login(
            LoginDto dto)
        {
            var user = await _userManager
                .FindByEmailAsync(dto.Email);

            if (user == null)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Invalid email or password."
                };
            }

            var result = await _signInManager
                .CheckPasswordSignInAsync(
                    user,
                    dto.Password,
                    false);

            if (!result.Succeeded)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Invalid email or password."
                };
            }

            var token = await GenerateToken(user);

            return new AuthResponseDto
            {
                Success = true,
                Message = "Login successful.",
                Token = token
            };
        }


        private async Task<string> GenerateToken(
            ApplicationUser user)
        {
            var roles = await _userManager
                .GetRolesAsync(user);

            var claims = new List<Claim>
        {
            new Claim(
                JwtRegisteredClaimNames.Sub,
                user.Id),

            new Claim(
                JwtRegisteredClaimNames.Email,
                user.Email!),

            new Claim(
                ClaimTypes.NameIdentifier,
                user.Id),

            new Claim(
                "clinicId",
                user.ClinicId.ToString())
        };


            foreach (var role in roles)
            {
                claims.Add(
                    new Claim(
                        ClaimTypes.Role,
                        role));
            }


            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    _configuration["Jwt:Key"]!));

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);


            var token = new JwtSecurityToken(
                issuer: null,
                audience: null,
                claims: claims,
                expires: DateTime.UtcNow.AddMonths(2),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler()
                .WriteToken(token);
        }
    }
}
