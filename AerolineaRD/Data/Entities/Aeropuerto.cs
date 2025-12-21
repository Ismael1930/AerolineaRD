using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AerolineaRD.Entity
{
    [Table("Aeropuerto")]
    public class Aeropuerto
    {
        public Aeropuerto()
        {
            VuelosOrigen = new HashSet<Vuelo>();
            VuelosDestino = new HashSet<Vuelo>();
            CapacidadVuelosPorHora = 10; // Valor por defecto
        }

        [Key]
        [Column("CodAeropuerto")]
        [MaxLength(10)]
        public string Codigo { get; set; } = null!;

        [Column("Nombre")]
        [MaxLength(50)]
        public string? Nombre { get; set; }

        [Column("Ciudad")]
        [MaxLength(50)]
        public string? Ciudad { get; set; }

        [Column("Pais")]
        [MaxLength(50)]
        public string? Pais { get; set; }

        /// <summary>
        /// Capacidad máxima de aterrizajes/despegues por hora
        /// </summary>
        [Column("CapacidadVuelosPorHora")]
        public int CapacidadVuelosPorHora { get; set; }

        [InverseProperty(nameof(Vuelo.Origen))]
        public ICollection<Vuelo> VuelosOrigen { get; set; }

        [InverseProperty(nameof(Vuelo.Destino))]
        public ICollection<Vuelo> VuelosDestino { get; set; }
    }
}
