namespace AerolineaRD.Data.DTOs
{
    public class AsientoResponseDto
    {
        public string Numero { get; set; } = null!;
        public int IdVuelo { get; set; }
        public string? Clase { get; set; }
        public string? Disponibilidad { get; set; }
    }
}