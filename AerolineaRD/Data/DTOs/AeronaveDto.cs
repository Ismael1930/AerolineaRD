namespace AerolineaRD.Data.DTOs
{
    public class CrearAeronaveDto
    {
        public string Matricula { get; set; } = null!;
        public string? Modelo { get; set; }
        public int Capacidad { get; set; }
        public string? Estado { get; set; }
    }

    public class ActualizarAeronaveDto
    {
        public string Matricula { get; set; } = null!;
        public string? Modelo { get; set; }
        public int? Capacidad { get; set; }
        public string? Estado { get; set; }
    }

    public class AeronaveResponseDto
    {
        public string Matricula { get; set; } = null!;
        public string? Modelo { get; set; }
        public int Capacidad { get; set; }
        public string? Estado { get; set; }
    }
}