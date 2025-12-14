namespace AerolineaRD.Data.DTOs
{
    public class RegisterDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; } = "Cliente";

        // Optional profile data to create Cliente and Pasajero
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public string? Telefono { get; set; }
        public string? Pasaporte { get; set; }
    }
}
