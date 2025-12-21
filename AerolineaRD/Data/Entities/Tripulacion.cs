using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AerolineaRD.Entity
{
    [Table("Tripulacion")]
    public class Tripulacion
    {
        public Tripulacion()
        {
            Vuelos = new HashSet<Vuelo>();
            TiempoDescansoMinutos = 480; // 8 horas por defecto
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
        public string? Rol { get; set; } // "Piloto", "Copiloto", "Sobrecargo", etc.

        [Column("Licencia")]
        [MaxLength(20)]
        public string? Licencia { get; set; }

        /// <summary>
        /// Tiempo mínimo de descanso requerido entre vuelos en minutos
        /// Por defecto: 480 minutos (8 horas según regulaciones internacionales)
        /// </summary>
        [Column("TiempoDescansoMinutos")]
        public int TiempoDescansoMinutos { get; set; }

        /// <summary>
        /// Certificaciones de tipos de aeronaves (separadas por coma)
        /// Ejemplo: "Boeing 737,Airbus A320"
        /// </summary>
        [Column("CertificacionesAeronave")]
        [MaxLength(200)]
        public string? CertificacionesAeronave { get; set; }

        // Relación muchos-a-muchos con Vuelo
        public ICollection<Vuelo> Vuelos { get; set; }
    }
}