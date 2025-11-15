namespace AerolineaRD.Data.DTOs
{
    public class BuscarVueloDto
    {
        public string? Origen { get; set; }
        public string? Destino { get; set; }
        public DateTime? FechaSalida { get; set; }
        public DateTime? FechaRegreso { get; set; }
        public int Adultos { get; set; } = 2;
        public int Ninos { get; set; } = 1;
        public int Habitaciones { get; set; } = 1;
        public string TipoViaje { get; set; } = "IdaYVuelta"; // "IdaYVuelta", "SoloIda"
        public string? Clase { get; set; } // "Economica", "0Maletas", etc.
    }
}
