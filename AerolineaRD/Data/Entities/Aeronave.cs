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
            Asientos = new HashSet<Asiento>();
            TiempoPreparacionMinutos = 120; // Valor por defecto: 2 horas
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

        /// <summary>
        /// Tiempo mínimo de preparación entre vuelos en minutos (limpieza, carga de combustible, embarque)
        /// Por defecto: 120 minutos (2 horas)
        /// </summary>
        [Column("TiempoPreparacionMinutos")]
        public int TiempoPreparacionMinutos { get; set; }

        [InverseProperty(nameof(Vuelo.Aeronave))]
        public ICollection<Vuelo> Vuelos { get; set; }

        [InverseProperty(nameof(Asiento.Aeronave))]
        public ICollection<Asiento> Asientos { get; set; }
    }
}