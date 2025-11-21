using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AerolineaRD.Entity
{
    [Table("Tripulacion")]
    public class Tripulacion
    {
        public Tripulacion()
        {
            Vuelos = new HashSet<Vuelo>(); // Solo la colección
        }

        [Key]
        [Column("IdTripulacion")]
        public int Id { get; set; }

        [Column("Nombre")]
        [MaxLength(50)]
        public string? Nombre { get; set; }

        [Column("Apellido")]
        [MaxLength(50)]
        public string? Apellido { get; set; }

        [Column("Rol")]
        [MaxLength(30)]
        public string? Rol { get; set; } // "Piloto", "Copiloto", "Azafata", etc.

        [Column("Licencia")]
        [MaxLength(20)]
        public string? Licencia { get; set; }

        // Relación muchos-a-muchos con Vuelo
        public ICollection<Vuelo> Vuelos { get; set; }
    }
}