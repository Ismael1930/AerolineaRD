using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AerolineaRD.Entity
{
    /// <summary>
    /// Tabla intermedia: Relación muchos a muchos entre Equipo y Personal
    /// </summary>
    [Table("EquipoPersonal")]
    public class EquipoPersonal
  {
      [Key]
        [Column("IdEquipoPersonal")]
        public int Id { get; set; }

        [Column("IdEquipo")]
      public int IdEquipo { get; set; }

    [Column("IdPersonal")]
        public int IdPersonal { get; set; }

        [Column("FechaAsignacion")]
     public DateTime FechaAsignacion { get; set; } = DateTime.Now;

        [Column("Activo")]
        public bool Activo { get; set; } = true;

        // Navegación
        [ForeignKey(nameof(IdEquipo))]
        public Equipo Equipo { get; set; } = null!;

        [ForeignKey(nameof(IdPersonal))]
        public Personal Personal { get; set; } = null!;
    }
}
