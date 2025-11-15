
namespace AerolineaRD.Data.DTOs
{
    public class VueloResponseDto
    {
        public int Id { get; set; }
        public string? NumeroVuelo { get; set; }
        public DateTime Fecha { get; set; }
        public string? OrigenCodigo { get; set; }
        public string? OrigenNombre { get; set; }
        public string? OrigenCiudad { get; set; }
        public string? DestinoCodigo { get; set; }
        public string? DestinoNombre { get; set; }
        public string? DestinoCiudad { get; set; }
        public int AsientosDisponibles { get; set; }
        public List<string>? ClasesDisponibles { get; set; }
    }
}