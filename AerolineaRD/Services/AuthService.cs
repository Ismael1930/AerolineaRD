using AerolineaRD.Data.DTOs;
using AerolineaRD.Services.interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public class AuthService : IAuthService
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _configuration;

    public AuthService(UserManager<IdentityUser> userManager,
                       RoleManager<IdentityRole> roleManager,
                       IConfiguration configuration)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
    }

    public async Task<IdentityResult> RegisterAsync(string email, string password, string role = "Cliente")
    {
        var user = new IdentityUser { UserName = email, Email = email };
        var result = await _userManager.CreateAsync(user, password);

        if (result.Succeeded)
        {
            if (!await _roleManager.RoleExistsAsync(role))
                await _roleManager.CreateAsync(new IdentityRole(role));

            await _userManager.AddToRoleAsync(user, role);
        }

        return result;
    }

    public async Task<LoginResponseDto?> LoginAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user != null && await _userManager.CheckPasswordAsync(user, password))
        {
            var roles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName ?? ""),
                new Claim(ClaimTypes.Email, user.Email ?? "")
            };

            foreach (var role in roles)
                claims.Add(new Claim(ClaimTypes.Role, role));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: creds);

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return new LoginResponseDto
            {
                Token = tokenString,
                Email = user.Email ?? "",
                UserName = user.UserName ?? "",
                UserId = user.Id,
                Roles = roles.ToList()
            };
        }

        return null;
    }

    public async Task<IdentityResult> CambiarContrasenaAsync(string userId, CambiarContrasenaDto dto)
    {
        // Validar que las contraseñas nuevas coincidan
        if (dto.NuevaContrasena != dto.ConfirmarContrasena)
        {
            var error = new IdentityError
            {
                Code = "PasswordMismatch",
                Description = "La nueva contraseña y la confirmación no coinciden."
            };
            return IdentityResult.Failed(error);
        }

        // Buscar el usuario
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            var error = new IdentityError
            {
                Code = "UserNotFound",
                Description = "Usuario no encontrado."
            };
            return IdentityResult.Failed(error);
        }

        // Verificar contraseña actual
        var isCurrentPasswordValid = await _userManager.CheckPasswordAsync(user, dto.ContrasenaActual);
        if (!isCurrentPasswordValid)
        {
            var error = new IdentityError
            {
                Code = "InvalidCurrentPassword",
                Description = "La contraseña actual es incorrecta."
            };
            return IdentityResult.Failed(error);
        }

        // Cambiar contraseña
        var result = await _userManager.ChangePasswordAsync(user, dto.ContrasenaActual, dto.NuevaContrasena);
        return result;
    }

    public async Task<IdentityResult> ResetPasswordAsync(ResetPasswordDto dto)
    {
        // Validar que las contraseñas nuevas coincidan
        if (dto.NuevaContrasena != dto.ConfirmarContrasena)
        {
            var error = new IdentityError
            {
                Code = "PasswordMismatch",
                Description = "La nueva contraseña y la confirmación no coinciden."
            };
            return IdentityResult.Failed(error);
        }

        // Buscar el usuario por email
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null)
        {
            var error = new IdentityError
            {
                Code = "UserNotFound",
                Description = "No se encontró un usuario con ese email."
            };
            return IdentityResult.Failed(error);
        }

        // Generar token de reset (necesario para ChangePasswordAsync sin contraseña actual)
        var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

        // Resetear la contraseña usando el token
        var result = await _userManager.ResetPasswordAsync(user, resetToken, dto.NuevaContrasena);
        return result;
    }
}
