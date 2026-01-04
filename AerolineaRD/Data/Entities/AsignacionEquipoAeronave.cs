using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AerolineaRD.Entity
{
    /// <summary>
    /// Asignación de un equipo a una aeronave específica
    /// Solo puede haber UNA asignación activa por aeronave
    /// </summary>
    [Table("AsignacionEquipoAeronave")]
  public class AsignacionEquipoAeronave
 {
        [Key]
        [Column("IdAsignacion")]
  public int Id { get; set; }

        [Column("IdEquipo")]
        public int IdEquipo { get; set; }

        [Column("Matricula")]
     [MaxLength(15)]
        public string Matricula { get; set; } = null!;

        [Column("FechaAsignacion")]
    public DateTime FechaAsignacion { get; set; } = DateTime.Now;

        [Column("FechaDesasignacion")]
        public DateTime? FechaDesasignacion { get; set; }

      [Column("Activa")]
        public bool Activa { get; set; } = true;
        // Solo puede haber UNA asignación activa por aeronave

        [Column("Observaciones")]
        [MaxLength(500)]
      public string? Observaciones { get; set; }

        // Navegación
        [ForeignKey(nameof(IdEquipo))]
    public Equipo Equipo { get; set; } = null!;

        [ForeignKey(nameof(Matricula))]
      public Aeronave Aeronave { get; set; } = null!;
    }
}
