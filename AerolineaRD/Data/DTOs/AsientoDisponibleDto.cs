namespace AerolineaRD.Data.DTOs
{
    public class AsientoDisponibleDto
    {
   public string Numero { get; set; } = null!; // "12A"
        public string Clase { get; set; } = null!; // "Economica"
   public bool Disponible { get; set; } // true si está libre
  public int Fila { get; set; } // 12
        public string Columna { get; set; } = null!; // "A"
    }

    public class ObtenerAsientosRequest
    {
        public int IdVuelo { get; set; }
        public string Clase { get; set; } = null!; // "Economica", "Ejecutiva", "Primera"
    }
}
