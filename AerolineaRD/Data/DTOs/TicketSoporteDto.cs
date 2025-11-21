namespace AerolineaRD.Data.DTOs
{
    public class CrearTicketDto
    {
        public int IdCliente { get; set; }
        public string? Asunto { get; set; }
        public string? Descripcion { get; set; }
        public string? Prioridad { get; set; }
    }

    public class ActualizarTicketDto
    {
        public int IdTicket { get; set; }
        public string? Estado { get; set; }
    }

    public class TicketSoporteResponseDto
    {
        public int Id { get; set; }
        public int IdCliente { get; set; }
        public string? ClienteNombre { get; set; }
        public string? Asunto { get; set; }
        public string? Descripcion { get; set; }
        public string? Estado { get; set; }
        public string? Prioridad { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaCierre { get; set; }
    }
}