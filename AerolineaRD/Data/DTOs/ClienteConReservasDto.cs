namespace AerolineaRD.Data.DTOs
{
    public class ClienteConReservasDto
    {
        public int Id { get; set; }
        public string? Nombre { get; set; }
        public string? Email { get; set; }
        public string? Telefono { get; set; }
        public int TotalReservas { get; set; }
        public List<ReservaSimpleDto>? Reservas { get; set; }
    }

    public class ReservaSimpleDto
    {
        public string? Codigo { get; set; }
        public DateTime FechaReserva { get; set; }
        public string? Estado { get; set; }
        public decimal PrecioTotal { get; set; }
    }
}