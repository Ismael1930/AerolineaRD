using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AerolineaRD.Entity
{
    [Table("Pasajero")]
    public class Pasajero
    {
        public Pasajero()
        {
            Reservas = new HashSet<Reserva>();
            Equipajes = new HashSet<Equipaje>();
        }

        [Key]
        [Column("IdPasajero")]
        public int Id { get; set; }

        [Column("Nombre")]
        [MaxLength(50)]
        public string? Nombre { get; set; }

        [Column("Apellido")]
        [MaxLength(50)]
        public string? Apellido { get; set; }

        [Column("Pasaporte")]
        [MaxLength(20)]
        public string? Pasaporte { get; set; }

        [InverseProperty(nameof(Reserva.Pasajero))]
        public ICollection<Reserva> Reservas { get; set; }

        [InverseProperty(nameof(Equipaje.Pasajero))]
        public ICollection<Equipaje> Equipajes { get; set; }
    }
}
