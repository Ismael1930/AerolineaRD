using AerolineaRD.Data.DTOs;
using AerolineaRD.Services.interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AerolineaRD.Controllers
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

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var result = await _authService.RegisterAsync(dto.Email, dto.Password, dto.Role);
            if (result.Succeeded)
                return Ok(new { message = $"Usuario registrado con rol {dto.Role}" });

            return BadRequest(result.Errors);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var token = await _authService.LoginAsync(dto.Email, dto.Password);
            if (token == null)
                return Unauthorized(new { message = "Credenciales inválidas" });

            return Ok(new { token });
        }
    }
}
