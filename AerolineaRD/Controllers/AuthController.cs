using AerolineaRD.Data.DTOs;
using AerolineaRD.Services.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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

        /// <summary>
        /// Cambiar contraseña del usuario autenticado
        /// </summary>
        [HttpPost("cambiar-contrasena")]
        [Authorize] // Requiere usuario autenticado
        public async Task<IActionResult> CambiarContrasena([FromBody] CambiarContrasenaDto dto)
        {
            try
            {
                // Obtener el ID del usuario desde el token JWT
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                    return BadRequest(new { success = false, message = "Usuario no identificado" });

                var result = await _authService.CambiarContrasenaAsync(userId, dto);

                if (result.Succeeded)
                {
                    return Ok(new { success = true, message = "Contraseña cambiada exitosamente" });
                }

                return BadRequest(new
                {
                    success = false,
                    message = "Error al cambiar la contraseña",
                    errors = result.Errors.Select(e => e.Description).ToList()
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Error interno del servidor", error = ex.Message });
            }
        }

        /// <summary>
        /// Resetear contraseña sin autenticación
        /// </summary>
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            try
            {
                var result = await _authService.ResetPasswordAsync(dto);

                if (result.Succeeded)
                {
                    return Ok(new { success = true, message = "Contraseña restablecida exitosamente" });
                }

                return BadRequest(new
                {
                    success = false,
                    message = "Error al restablecer la contraseña",
                    errors = result.Errors.Select(e => e.Description).ToList()
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Error interno del servidor", error = ex.Message });
            }
        }
    }
}
