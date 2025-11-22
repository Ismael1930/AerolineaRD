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
                return Ok(new { success = true, message = $"Usuario registrado con rol {dto.Role}" });

            return BadRequest(new { success = false, errors = result.Errors });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var loginResponse = await _authService.LoginAsync(dto.Email, dto.Password);
            if (loginResponse == null)
                return Unauthorized(new { success = false, message = "Credenciales inválidas" });

            return Ok(new
            {
                success = true,
                token = loginResponse.Token,
                email = loginResponse.Email,
                userName = loginResponse.UserName,
                userId = loginResponse.UserId,
                roles = loginResponse.Roles
            });
        }
    }
}
