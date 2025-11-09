using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AerolineaRD.Entity
{
    [Table("Asiento")]
    public class Asiento
    {
        [Key]
        [Column("NumAsiento")]
        [MaxLength(5)]
        public string Numero { get; set; } = null!;

        [Column("IdVuelo")]
        public int IdVuelo { get; set; }

        [Column("Clase")]
        [MaxLength(20)]
        public string? Clase { get; set; }

        [Column("Disponibilidad")]
        [MaxLength(10)]
        public string? Disponibilidad { get; set; }

        [ForeignKey(nameof(IdVuelo))]
        [InverseProperty(nameof(Vuelo.Asientos))]
        public Vuelo? Vuelo { get; set; }
    }
}
