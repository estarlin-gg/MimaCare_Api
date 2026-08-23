using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MimaCare.Application.Dtos.Auth;
using MimaCare.Application.Interface;

namespace MimaCare.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        [HttpPost("register-clinic")]
        public async Task<ActionResult<AuthResponseDto>> Register (RegisterClinicDto registerClinicDto)
        {
            var res = await _authService.Register(registerClinicDto);

            return Ok(res);
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login(LoginDto loginDto)
        {
            var res = await _authService.Login(loginDto);

            return Ok(res);
        }

        [HttpPut("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto dto)
        {
            await _authService.ChangePassword(dto);

            return Ok("Contraseña actualizada correctamente.");
        }

        [HttpPost("forgot-password")]
        public async Task<ActionResult<ForgotPasswordResponseDto>> ForgotPassword(ForgotPasswordDto dto)
        {
            var res = await _authService.ForgotPassword(dto);

            return Ok(res);
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto dto)
        {
            await _authService.ResetPassword(dto);

            return Ok("Contraseña restablecida correctamente.");
        }

        [HttpPut("reset-password/{userId:guid}")]
        [Authorize(Roles = "Clinic,Super")]
        public async Task<IActionResult> AdminResetPassword(Guid userId, AdminResetPasswordDto dto)
        {
            await _authService.AdminResetPassword(userId, dto);

            return Ok("Contraseña restablecida correctamente.");
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _authService.Logout();

            return Ok("Sesión cerrada correctamente.");
        }
    }
}
