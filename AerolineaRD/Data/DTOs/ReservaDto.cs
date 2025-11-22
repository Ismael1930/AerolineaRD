namespace AerolineaRD.Data.DTOs
{
    public class CrearReservaDto
    {
        public int IdPasajero { get; set; }
        public int IdVuelo { get; set; }
        public int IdCliente { get; set; }
        public string? NumAsiento { get; set; }
        public string? Clase { get; set; } // "Economica", "Ejecutiva", "Primera"
        public string? MetodoPago { get; set; }
        public decimal? PrecioTotal { get; set; } 
    }

    public class ModificarReservaDto
    {
        public string CodigoReserva { get; set; } = null!;
        public int? NuevoIdVuelo { get; set; }
        public string? NuevoNumAsiento { get; set; }
    }

    public class ReservaResponseDto
    {
        public string Codigo { get; set; } = null!;
        public string? PasajeroNombre { get; set; }
        public string? PasajeroApellido { get; set; }
        public string? NumeroVuelo { get; set; }
        public DateTime FechaVuelo { get; set; }
        public string? Origen { get; set; }
        public string? Destino { get; set; }
        public string? NumAsiento { get; set; }
        public string? Clase { get; set; }
        public DateTime FechaReserva { get; set; }
        public string? Estado { get; set; }
        public decimal PrecioTotal { get; set; }
        public FacturaResponseDto? Factura { get; set; }
    }
}