using MimaCare.Application.Dtos.Auth;

namespace MimaCare.Application.Interface
{
    public interface IAuthService
    {
        Task<AuthResponseDto> Login(LoginDto loginDto);

        Task<AuthResponseDto> Register(RegisterClinicDto registerDto);
    }
}
