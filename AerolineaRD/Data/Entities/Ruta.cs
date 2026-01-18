using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AerolineaRD.Entity
{
    /// <summary>
    /// Representa una ruta aérea entre dos aeropuertos con su duración estimada en minutos
    /// </summary>
    [Table("Ruta")]
    public class Ruta
    {
        [Key]
        [Column("IdRuta")]
        public int Id { get; set; }

        /// <summary>
        /// Código del aeropuerto de origen
        /// </summary>
        [Column("OrigenCodigo")]
        [MaxLength(10)]
        [Required]
        public string OrigenCodigo { get; set; } = null!;

        /// <summary>
        /// Código del aeropuerto de destino
        /// </summary>
        [Column("DestinoCodigo")]
        [MaxLength(10)]
        [Required]
        public string DestinoCodigo { get; set; } = null!;

        /// <summary>
        /// Duración estimada del vuelo en minutos
        /// </summary>
        [Column("DuracionMinutos")]
        [Required]
        public int DuracionMinutos { get; set; }

        /// <summary>
        /// Distancia aproximada en kilómetros (opcional)
        /// </summary>
        [Column("DistanciaKm")]
        public int? DistanciaKm { get; set; }

        /// <summary>
        /// Indica si la ruta está activa para programar vuelos
        /// </summary>
        [Column("Activa")]
        public bool Activa { get; set; } = true;

        // Navegación
        [ForeignKey(nameof(OrigenCodigo))]
        public Aeropuerto? Origen { get; set; }

        [ForeignKey(nameof(DestinoCodigo))]
        public Aeropuerto? Destino { get; set; }
    }
}
