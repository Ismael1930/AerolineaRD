namespace AerolineaRD.Data.DTOs
{
    public class LoginResponseDto
    {
        public string Token { get; set; } = null!;
        public string Email { get; set; } = null!;
   public string UserName { get; set; } = null!;
        public string UserId { get; set; } = null!;
        public List<string> Roles { get; set; } = new();
    }
}
