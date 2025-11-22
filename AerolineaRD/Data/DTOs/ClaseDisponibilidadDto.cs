namespace AerolineaRD.Data.DTOs
{
    public class ClaseDisponibilidadDto
    {
        public string Clase { get; set; } = null!; // "Economica", "Ejecutiva", "Primera"
        public int AsientosDisponibles { get; set; }
        public decimal Precio { get; set; }
    }
}
