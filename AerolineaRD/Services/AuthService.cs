using AerolineaRD.Data;
using AerolineaRD.Data.DTOs;
using AerolineaRD.Services.interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AerolineaRD.Repositories.interfaces;
using AerolineaRD.Entity;
using Microsoft.Data.Sqlite;

public class AuthService : IAuthService
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _configuration;
    private readonly IClienteRepository _clienteRepository;
    private readonly IPasajeroRepository _pasajeroRepository;
    private readonly AppDbContext _context;

    public AuthService(UserManager<IdentityUser> userManager,
                       RoleManager<IdentityRole> roleManager,
                       IConfiguration configuration,
                       IClienteRepository clienteRepository,
                       IPasajeroRepository pasajeroRepository,
                       AppDbContext context)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
        _clienteRepository = clienteRepository;
        _pasajeroRepository = pasajeroRepository; // keep original field name
        _context = context;
    }

    public async Task<IdentityResult> RegisterAsync(RegisterDto dto)
    {
        IdentityUser? user = null;
        // Start a transaction for the whole flow (including Identity operations) to ensure single connection
        using var transaction = await BeginTransactionWithRetryAsync(5);
        try
        {
            // Create Identity user inside the transaction
            user = new IdentityUser { UserName = dto.Email, Email = dto.Email };
            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
            {
                await transaction.RollbackAsync();
                return result;
            }

            // Ensure role exists and assign
            if (!await _roleManager.RoleExistsAsync(dto.Role))
                await _roleManager.CreateAsync(new IdentityRole(dto.Role));

            await _userManager.AddToRoleAsync(user, dto.Role);

            // Crear Cliente vinculado al User
            var cliente = new Cliente
            {
                Nombre = dto.Nombre ?? dto.Email,
                Email = dto.Email,
                Telefono = dto.Telefono,
                UserId = user.Id
            };

            await _clienteRepository.AddAsync(cliente);
            await _clienteRepository.SaveAsync();

            // Crear Pasajero vinculado al Cliente (si se proporcionó pasaporte o nombre)
            if (!string.IsNullOrEmpty(dto.Pasaporte) || !string.IsNullOrEmpty(dto.Nombre) || !string.IsNullOrEmpty(dto.Apellido))
            {
                var pasajero = new Pasajero
                {
                    Nombre = dto.Nombre,
                    Apellido = dto.Apellido,
                    Pasaporte = dto.Pasaporte,
                    IdCliente = cliente.Id
                };

                await _pasajeroRepository.AddAsync(pasajero);
                await _pasajeroRepository.SaveAsync();
            }

            await transaction.CommitAsync();
            return IdentityResult.Success;
        }
        catch (Exception)
        {
            try { await transaction.RollbackAsync(); } catch { }
            // Best-effort cleanup: delete created Identity user
            if (user != null)
            {
                try { await _userManager.DeleteAsync(user); } catch { }
            }
            throw;
        }
    }

    private async Task<Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction> BeginTransactionWithRetryAsync(int maxRetries)
    {
        int attempt = 0;
        while (true)
        {
            try
            {
                return await _context.Database.BeginTransactionAsync();
            }
            catch (SqliteException ex) when (ex.SqliteErrorCode == 5) // SQLITE_BUSY
            {
                attempt++;
                if (attempt >= maxRetries)
                    throw;
                await Task.Delay(150 * attempt);
            }
        }
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
