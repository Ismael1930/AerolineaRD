namespace AerolineaRD.Data.DTOs
{
    public class ResetPasswordDto
    {
        public string Email { get; set; } = null!;
   public string NuevaContrasena { get; set; } = null!;
 public string ConfirmarContrasena { get; set; } = null!;
    }
}