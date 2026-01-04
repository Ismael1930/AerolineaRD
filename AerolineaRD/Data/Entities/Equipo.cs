using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AerolineaRD.Entity
{
    /// <summary>
  /// Representa un equipo de tripulación completo
    /// </summary>
    [Table("Equipo")]
    public class Equipo
  {
        [Key]
 [Column("IdEquipo")]
        public int Id { get; set; }

        [Column("Nombre")]
        [MaxLength(100)]
        public string Nombre { get; set; } = null!;
      // Ej: "Equipo Alpha", "Tripulación 1"

        [Column("Codigo")]
      [MaxLength(20)]
        public string Codigo { get; set; } = null!;
        // Ej: "EQ-001", "TRIP-A"

        [Column("Estado")]
        [MaxLength(20)]
        public string Estado { get; set; } = "Disponible";
        // "Disponible", "En Servicio", "Descanso", "Incompleto"

   [Column("FechaCreacion")]
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        [Column("UltimoVueloFin")]
        public DateTime? UltimoVueloFin { get; set; }

        [Column("DisponibleDesde")]
        public DateTime? DisponibleDesde { get; set; }
     // Calculado: UltimoVueloFin + tiempo de descanso

      [Column("Activo")]
        public bool Activo { get; set; } = true;

        // Navegación
        public ICollection<EquipoPersonal> EquiposPersonal { get; set; } = new List<EquipoPersonal>();
        public ICollection<AsignacionEquipoAeronave> AsignacionesAeronave { get; set; } = new List<AsignacionEquipoAeronave>();
    }
}
