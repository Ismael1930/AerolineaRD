using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AerolineaRD.Entity
{
    [Table("Equipaje")]
    public class Equipaje
    {
        [Key]
        [Column("NumEquipaje")]
        [MaxLength(10)]
        public string Numero { get; set; } = null!;

        [Column("IdPasajero")]
        public int IdPasajero { get; set; }

        [Column("Peso", TypeName = "decimal(5,2)")]
        public decimal Peso { get; set; }

        [Column("Tipo")]
        [MaxLength(20)]
        public string? Tipo { get; set; }

        [ForeignKey(nameof(IdPasajero))]
        [InverseProperty(nameof(Pasajero.Equipajes))]
        public Pasajero? Pasajero { get; set; }
    }
}
