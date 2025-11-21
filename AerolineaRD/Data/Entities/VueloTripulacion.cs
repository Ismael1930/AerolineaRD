using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AerolineaRD.Entity
{
    [Table("VueloTripulacion")]
    public class VueloTripulacion
    {
        [Key]
        [Column("IdVueloTripulacion")]
        public int Id { get; set; }  

        [Column("IdVuelo")]
        public int IdVuelo { get; set; }

        [Column("IdTripulacion")]
        public int IdTripulacion { get; set; }

        [ForeignKey(nameof(IdVuelo))]
        public Vuelo? Vuelo { get; set; }

        [ForeignKey(nameof(IdTripulacion))]
        public Tripulacion? Tripulacion { get; set; }
    }
}