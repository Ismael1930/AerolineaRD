using Microsoft.AspNetCore.Identity;

namespace AerolineaRD.Services.interfaces
{
    public interface IAuthService
    {
        Task<IdentityResult> RegisterAsync(string email, string password, string role = "Cliente");
        Task<string?> LoginAsync(string email, string password);
    }
}

