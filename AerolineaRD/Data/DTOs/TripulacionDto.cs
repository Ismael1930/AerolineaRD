namespace AerolineaRD.Data.DTOs
{
    public class CrearTripulacionDto
    {
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public string? Rol { get; set; }
        public string? Licencia { get; set; }
    }

    public class TripulacionDto
    {
        public int Id { get; set; }
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public string? Rol { get; set; }
        public string? Licencia { get; set; }
    }
}