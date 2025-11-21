using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AerolineaRD.Entity
{
    [Table("EstadoVuelo")]
    public class EstadoVuelo
    {
        [Key]
        [Column("IdEstado")]
        public int Id { get; set; }

        [Column("IdVuelo")]
        public int IdVuelo { get; set; }

        [Column("Estado")]
        [MaxLength(30)]
        public string? Estado { get; set; } // "Programado", "Embarcando", "En Vuelo", "Aterrizado", "Retrasado", "Cancelado"

        [Column("HoraSalida")]
        public DateTime? HoraSalida { get; set; }

        [Column("HoraLlegada")]
        public DateTime? HoraLlegada { get; set; }

        [Column("HoraSalidaProgramada")]
        public DateTime HoraSalidaProgramada { get; set; }

        [Column("HoraLlegadaProgramada")]
        public DateTime HoraLlegadaProgramada { get; set; }

        [Column("Puerta")]
        [MaxLength(10)]
        public string? Puerta { get; set; }

        [Column("Observaciones")]
        [MaxLength(200)]
        public string? Observaciones { get; set; }

        [ForeignKey(nameof(IdVuelo))]
        public Vuelo? Vuelo { get; set; }
    }
}