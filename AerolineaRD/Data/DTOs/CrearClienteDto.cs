namespace AerolineaRD.Data.DTOs
{
    public class CrearClienteDto
    {
        public string Nombre { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Telefono { get; set; }
        public string? UserId { get; set; }
    }
}