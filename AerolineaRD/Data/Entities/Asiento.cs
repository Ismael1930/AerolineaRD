using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AerolineaRD.Entity
{
    [Table("Asiento")]
    public class Asiento
    {
        [Key]
        [Column("NumAsiento")]
        [MaxLength(10)] // Aumentado para incluir matrícula: "HI1001-1A"
        public string Numero { get; set; } = null!;

        [Column("Matricula")]
        [MaxLength(15)]
        public string Matricula { get; set; } = null!; // ⬅️ CAMBIO: Ahora pertenece a aeronave

        [Column("NumeroAsiento")] // El número real del asiento: "1A", "12C"
        [MaxLength(5)]
        public string? NumeroAsiento { get; set; }

        [Column("Clase")]
        [MaxLength(20)]
        public string? Clase { get; set; } // "Primera", "Ejecutiva", "Economica"

        [ForeignKey(nameof(Matricula))]
        [InverseProperty(nameof(Aeronave.Asientos))]
        public Aeronave? Aeronave { get; set; }
    }
}
