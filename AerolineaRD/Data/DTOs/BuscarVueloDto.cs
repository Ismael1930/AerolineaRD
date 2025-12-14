namespace AerolineaRD.Data.DTOs
{
    public class BuscarVueloDto
    {
        public string? Origen { get; set; }
        public string? Destino { get; set; }

        // Existing single-date properties kept for compatibility
        public DateTime? FechaSalidaInicio { get; set; }
        public DateTime? FechaRegresoInicio { get; set; }

        // New: date range support (optional)
        public DateTime? FechaSalidaFin { get; set; }
        public DateTime? FechaRegresoFin { get; set; }

        public int Adultos { get; set; } = 1;
        public int Ninos { get; set; } = 0;
        public int Habitaciones { get; set; } = 1;
        public string TipoViaje { get; set; } = "SoloIda"; // "IdaYVuelta" / "SoloIda"
        public string? Clase { get; set; }
    }
}
