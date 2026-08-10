using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MimaCare.Application.Common.Exceptions;
using MimaCare.Application.Dtos.Auth;
using MimaCare.Application.Interface;
using MimaCare.Domain.Entities;
using MimaCare.Domain.Interfaces;
using MimaCare.Infrastructure.Constants;
using MimaCare.Infrastructure.Repositories.Abstraction;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MimaCare.Application.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser>
        _userManager;

    private readonly SignInManager<ApplicationUser>
        _signInManager;

    private readonly IConfiguration
        _configuration;

    private readonly IClinicRepository
        _clinicRepository;

    private readonly IUnitOfWork
        _unitOfWork;

    private readonly IHttpContextAccessor
        _httpContextAccessor;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IConfiguration configuration,
        IClinicRepository clinicRepository,
        IUnitOfWork unitOfWork,
        IHttpContextAccessor httpContextAccessor)
    {
        _userManager = userManager;

        _signInManager = signInManager;

        _configuration = configuration;

        _clinicRepository = clinicRepository;

        _unitOfWork = unitOfWork;

        _httpContextAccessor = httpContextAccessor;
    }

    // =====================================================
    // REGISTER
    // =====================================================

    public async Task<AuthResponseDto>
        Register(RegisterClinicDto dto)
    {
        await _unitOfWork
            .BeginTransactionAsync();

        try
        {
            var emailExists = await _userManager
                .FindByEmailAsync(dto.Email);

            if (emailExists != null)
            {
                throw new ConflictException(
                    "Email already exists.");
            }


            var clinic = dto.Adapt<Clinic>();

            await _clinicRepository
                .CreateClinic(clinic);

  
            var user = dto
                .Adapt<ApplicationUser>();

            user.UserName = dto.Email;

            user.Email = dto.Email;

            user.ClinicId = clinic.Id;

            user.IsActive = true;

            var result = await _userManager
                .CreateAsync(
                    user,
                    dto.Password);

            if (!result.Succeeded)
            {
                throw new BadRequestException(
                    string.Join(
                        ", ",
                        result.Errors
                            .Select(x =>
                                x.Description)));
            }


            var roleResult = await _userManager
                .AddToRoleAsync(
                    user,
                    Roles.Clinic);

            if (!roleResult.Succeeded)
            {
                throw new BadRequestException(
                    string.Join(
                        ", ",
                        roleResult.Errors
                            .Select(x =>
                                x.Description)));
            }

            await _unitOfWork
                .CommitAsync();

            var token = await GenerateToken(user);

            return new AuthResponseDto
            {
                FullName = clinic.Name,
                ClinicName = clinic.Name,
                Role = Roles.Clinic,
                Token = token,
                Message = "Clinic registered successfully."
            };
        }
        catch
        {
            await _unitOfWork
                .RollbackAsync();

            throw;
        }
    }


    public async Task<AuthResponseDto>
        Login(LoginDto dto)
    {
        var user = await _userManager
            .FindByEmailAsync(dto.Email);

        if (user == null)
        {
            throw new UnauthorizedException(
                "Invalid email or password.");
        }

        var result = await _signInManager
            .CheckPasswordSignInAsync(
                user,
                dto.Password,
                false);

        if (!result.Succeeded)
        {
            throw new UnauthorizedException(
                "Invalid email or password.");
        }

        var token =
            await GenerateToken(user);

        var roles = await _userManager
            .GetRolesAsync(user);

        var clinic = await _clinicRepository
            .GetByIdAsync(user.ClinicId);

        var fullName = string.IsNullOrWhiteSpace(user.FirstName)
            ? clinic?.Name ?? user.Email
            : $"{user.FirstName} {user.LastName}".Trim();

        return new AuthResponseDto
        {
            FullName = fullName,
            ClinicName = clinic?.Name,
            Role = roles.FirstOrDefault(),
            Token = token,
            Message = "Login successful."
        };
    }


    public async Task ChangePassword(ChangePasswordDto dto)
    {
        var userId = _httpContextAccessor
            .HttpContext?
            .User?
            .FindFirst(ClaimTypes.NameIdentifier)?
            .Value;

        if (string.IsNullOrEmpty(userId))
        {
            throw new UnauthorizedException("User not found.");
        }

        var user = await _userManager
            .FindByIdAsync(userId);

        if (user == null)
        {
            throw new UnauthorizedException("User not found.");
        }

        var result = await _userManager
            .ChangePasswordAsync(
                user,
                dto.CurrentPassword,
                dto.NewPassword);

        if (!result.Succeeded)
        {
            var message = result.Errors
                .Any(e => e.Code == "PasswordMismatch")
                ? "La contraseña actual no es correcta."
                : string.Join(", ", result.Errors.Select(e => e.Description));

            throw new BadRequestException(message);
        }
    }


    private async Task<string>
        GenerateToken(
            ApplicationUser user)
    {
        var roles = await _userManager
            .GetRolesAsync(user);

        var claims = new List<Claim>
        {
            new Claim(
                JwtRegisteredClaimNames.Sub,
                user.Id.ToString()),

            new Claim(
                JwtRegisteredClaimNames.Email,
                user.Email!),

            new Claim(
                ClaimTypes.NameIdentifier,
                user.Id.ToString()),

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

        var key =
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    _configuration["Jwt:Key"]!));

        var credentials =
            new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

        var token =
            new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow
                    .AddMonths(2),
                signingCredentials:
                    credentials);

        return new JwtSecurityTokenHandler()
            .WriteToken(token);
    }
}