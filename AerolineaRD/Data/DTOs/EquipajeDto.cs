namespace AerolineaRD.Data.DTOs
{
    public class CrearEquipajeDto
    {
        public int IdPasajero { get; set; }
        public decimal Peso { get; set; }
        public string? Tipo { get; set; } // "Mano", "Bodega"
    }

    public class EquipajeResponseDto
    {
        public string Numero { get; set; } = null!;
        public int IdPasajero { get; set; }
        public string? PasajeroNombre { get; set; }
        public decimal Peso { get; set; }
        public string? Tipo { get; set; }
    }
}