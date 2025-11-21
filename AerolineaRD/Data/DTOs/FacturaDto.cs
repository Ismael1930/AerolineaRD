namespace AerolineaRD.Data.DTOs
{
    public class FacturaResponseDto
    {
        public string Codigo { get; set; } = null!;
        public string? CodReserva { get; set; }
        public decimal Monto { get; set; }
        public string? MetodoPago { get; set; }
        public DateTime FechaEmision { get; set; }
        public string? EstadoPago { get; set; }
    }

    public class PagarFacturaDto
    {
        public string CodigoFactura { get; set; } = null!;
        public string MetodoPago { get; set; } = null!; // "Tarjeta", "Transferencia", "Efectivo"
    }
}