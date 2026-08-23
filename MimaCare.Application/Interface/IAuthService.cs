using MimaCare.Application.Dtos.Auth;
using MimaCare.Domain.Common;

namespace MimaCare.Application.Interface
{
    public interface IAuthService
    {
        Task<AuthResponseDto> Login(LoginDto loginDto);

        Task<AuthResponseDto> Register(RegisterClinicDto registerDto);

        Task ChangePassword(ChangePasswordDto dto);

        Task<ForgotPasswordResponseDto> ForgotPassword(ForgotPasswordDto dto);

        Task ResetPassword(ResetPasswordDto dto);

        Task AdminResetPassword(Guid userId, AdminResetPasswordDto dto);

        Task Logout();
    }
}
