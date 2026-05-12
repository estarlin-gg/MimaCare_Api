using MimaCare.Application.Dtos.Auth;

namespace MimaCare.Application.Services.Abstraction
{
    public interface IAuthService
    {
        Task<AuthResponseDto> Login(LoginDto loginDto);

        Task<AuthResponseDto> Register(RegisterClinicDto registerDto);
    }
}
