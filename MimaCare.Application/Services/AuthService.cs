using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MimaCare.Application.Common;
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

    private readonly IRevokedTokenRepository
        _revokedTokenRepository;

    private readonly IOptions<PasswordResetOptions>
        _passwordResetOptions;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IConfiguration configuration,
        IClinicRepository clinicRepository,
        IUnitOfWork unitOfWork,
        IHttpContextAccessor httpContextAccessor,
        IRevokedTokenRepository revokedTokenRepository,
        IOptions<PasswordResetOptions> passwordResetOptions)
    {
        _userManager = userManager;

        _signInManager = signInManager;

        _configuration = configuration;

        _clinicRepository = clinicRepository;

        _unitOfWork = unitOfWork;

        _httpContextAccessor = httpContextAccessor;

        _revokedTokenRepository = revokedTokenRepository;

        _passwordResetOptions = passwordResetOptions;
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


    // =====================================================
    // FORGOT PASSWORD (self-service, token based)
    // =====================================================

    public async Task<ForgotPasswordResponseDto>
        ForgotPassword(ForgotPasswordDto dto)
    {
        var genericMessage =
            "If the email exists, a password reset token has been generated.";

        var user = await _userManager
            .FindByEmailAsync(dto.Email);

        if (user == null || !user.IsActive)
        {
            return new ForgotPasswordResponseDto
            {
                Message = genericMessage
            };
        }

        var resetToken = await _userManager
            .GeneratePasswordResetTokenAsync(user);

        var response = new ForgotPasswordResponseDto
        {
            Message = genericMessage
        };

        // No email provider is configured: expose the token
        // only in Development so the flow can be completed.
        if (_passwordResetOptions.Value.ExposeResetToken)
        {
            response.ResetToken = resetToken;
        }

        return response;
    }


    // =====================================================
    // RESET PASSWORD (self-service, token based)
    // =====================================================

    public async Task ResetPassword(ResetPasswordDto dto)
    {
        var user = await _userManager
            .FindByEmailAsync(dto.Email);

        if (user == null)
        {
            throw new BadRequestException(
                "Invalid or expired reset token.");
        }

        var result = await _userManager
            .ResetPasswordAsync(
                user,
                dto.Token,
                dto.NewPassword);

        if (!result.Succeeded)
        {
            throw new BadRequestException(
                string.Join(
                    ", ",
                    result.Errors
                        .Select(x =>
                            x.Description)));
        }
    }


    // =====================================================
    // ADMIN RESET PASSWORD (Clinic/Super force reset)
    // =====================================================

    public async Task AdminResetPassword(Guid userId, AdminResetPasswordDto dto)
    {
        var roles = GetUserRoles();

        if (!roles.Contains(Roles.Clinic) && !roles.Contains(Roles.Admin))
        {
            throw new ForbiddenException(
                "Only clinic admins can reset passwords.");
        }

        var callerClinicId =
            GetClinicId();

        var user = await _userManager
            .FindByIdAsync(userId.ToString());

        if (user == null || user.ClinicId != callerClinicId)
        {
            throw new NotFoundException(
                "User not found in this clinic.");
        }

        var resetToken = await _userManager
            .GeneratePasswordResetTokenAsync(user);

        var result = await _userManager
            .ResetPasswordAsync(
                user,
                resetToken,
                dto.NewPassword);

        if (!result.Succeeded)
        {
            throw new BadRequestException(
                string.Join(
                    ", ",
                    result.Errors
                        .Select(x =>
                            x.Description)));
        }
    }


    // =====================================================
    // LOGOUT (JWT revocation via blacklist)
    // =====================================================

    public async Task Logout()
    {
        var jti =
            GetJtiClaim();

        if (string.IsNullOrEmpty(jti))
        {
            throw new BadRequestException(
                "This session cannot be revoked server-side. Please sign in again.");
        }

        var expiresAt =
            ParseTokenExpiration() ?? DateTime.UtcNow.AddMonths(2);

        await _revokedTokenRepository
            .AddAsync(new RevokedToken
            {
                Jti = jti,
                ExpiresAt = expiresAt
            });

        // Opportunistic cleanup of already-expired rows.
        await _revokedTokenRepository
            .DeleteExpiredAsync();

        await _revokedTokenRepository
            .SaveChangesAsync();
    }

    private string? GetJtiClaim()
    {
        var user = _httpContextAccessor
            .HttpContext?
            .User;

        return user?.FindFirst(JwtRegisteredClaimNames.Jti)?.Value
            ?? user?.FindFirst(ClaimTypes.SerialNumber)?.Value;
    }

    private DateTime? ParseTokenExpiration()
    {
        var user = _httpContextAccessor
            .HttpContext?
            .User;

        var expValue =
            user?.FindFirst(JwtRegisteredClaimNames.Exp)?.Value
            ?? user?.FindFirst(ClaimTypes.Expiration)?.Value;

        if (long.TryParse(expValue, out var seconds))
        {
            return DateTimeOffset
                .FromUnixTimeSeconds(seconds)
                .UtcDateTime;
        }

        return null;
    }

    private Guid GetClinicId()
    {
        var claim = _httpContextAccessor.HttpContext?.User?.FindFirst("clinicId")?.Value;
        if (string.IsNullOrEmpty(claim) || !Guid.TryParse(claim, out var clinicId))
            throw new UnauthorizedException("Clinic not found.");
        return clinicId;
    }

    private List<string> GetUserRoles()
    {
        var claims = _httpContextAccessor.HttpContext?.User?.FindAll(ClaimTypes.Role);
        return claims?.Select(c => c.Value).ToList() ?? new List<string>();
    }


    private async Task<string>
        GenerateToken(
            ApplicationUser user)
    {
        var roles = await _userManager
            .GetRolesAsync(user);

        var jti =
            Guid.NewGuid().ToString();

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
                user.ClinicId.ToString()),

            // Stored under both types so it can be
            // read back deterministically regardless
            // of inbound claim mapping.
            new Claim(
                JwtRegisteredClaimNames.Jti,
                jti),

            new Claim(
                ClaimTypes.SerialNumber,
                jti)
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