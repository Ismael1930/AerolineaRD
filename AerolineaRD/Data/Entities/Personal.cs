using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AerolineaRD.Entity
{
    /// <summary>
    /// Representa un miembro individual del personal de vuelo
    /// </summary>
    [Table("Personal")]
    public class Personal
    {
  [Key]
    [Column("IdPersonal")]
 public int Id { get; set; }

 [Column("Nombre")]
      [MaxLength(50)]
        public string Nombre { get; set; } = null!;

        [Column("Apellido")]
        [MaxLength(50)]
        public string Apellido { get; set; } = null!;

        [Column("Rol")]
        [MaxLength(30)]
     public string Rol { get; set; } = null!;
        // "Piloto", "Copiloto", "Sobrecargo Jefe", "Sobrecargo"

        [Column("Licencia")]
        [MaxLength(20)]
        public string? Licencia { get; set; }

        [Column("CertificacionesAeronave")]
        [MaxLength(200)]
        public string? CertificacionesAeronave { get; set; }
// CSV: "Boeing 737,Boeing 787,Airbus A320"

        [Column("TiempoDescansoMinutos")]
        public int TiempoDescansoMinutos { get; set; } = 480; // 8 horas por defecto

        [Column("Estado")]
      [MaxLength(20)]
        public string Estado { get; set; } = "Disponible";
        // "Disponible", "En Servicio", "Descanso", "Incapacitado"

        [Column("UltimoVueloFin")]
        public DateTime? UltimoVueloFin { get; set; }
        // Para calcular si cumplió el tiempo de descanso

 [Column("FechaContratacion")]
      public DateTime FechaContratacion { get; set; } = DateTime.Today;

        [Column("Activo")]
    public bool Activo { get; set; } = true;

        // Navegación
        public ICollection<EquipoPersonal> EquiposPersonal { get; set; } = new List<EquipoPersonal>();
    }
}
