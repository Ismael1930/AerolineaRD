using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AerolineaRD.Entity
{
    [Table("Aeronave")]
    public class Aeronave
    {
        public Aeronave()
        {
            Vuelos = new HashSet<Vuelo>();
        }

        [Key]
        [Column("Matricula")]
        [MaxLength(15)]
        public string Matricula { get; set; } = null!;

        [Column("Modelo")]
        [MaxLength(50)]
        public string? Modelo { get; set; }

        [Column("Capacidad")]
        public int Capacidad { get; set; }

        [Column("Estado")]
        [MaxLength(20)]
        public string? Estado { get; set; } // "Operativa", "Mantenimiento", "Fuera de Servicio"

        [InverseProperty(nameof(Vuelo.Aeronave))]
        public ICollection<Vuelo> Vuelos { get; set; }
    }
}