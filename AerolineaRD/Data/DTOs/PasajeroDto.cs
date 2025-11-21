namespace AerolineaRD.Data.DTOs
{
    public class CrearPasajeroDto
    {
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public string? Pasaporte { get; set; }
    }

    public class PasajeroResponseDto
    {
        public int Id { get; set; }
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public string? Pasaporte { get; set; }
    }
}