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
    }
}
