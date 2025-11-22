using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AerolineaRD.Entity
{
    [Table("Vuelo")]
    public class Vuelo
    {
        public Vuelo()
        {
            Reservas = new HashSet<Reserva>();
            Tripulaciones = new HashSet<Tripulacion>();
        }

        [Key]
        [Column("IdVuelo")]
        public int Id { get; set; }

        [Column("NumeroVuelo")]
        [MaxLength(10)]
        public string? NumeroVuelo { get; set; }

        [Column("Fecha", TypeName = "date")]
        public DateTime Fecha { get; set; }

        [Column("HoraSalida")]
        public TimeSpan HoraSalida { get; set; }

        [Column("HoraLlegada")]
        public TimeSpan HoraLlegada { get; set; }

        [Column("Duracion")]
        public int Duracion { get; set; } // Minutos

        [Column("PrecioBase", TypeName = "decimal(10,2)")]
        public decimal PrecioBase { get; set; } // ⬅️ Este es el precio ECONÓMICO base

        [Column("Origen")]
        [MaxLength(10)]
        public string? OrigenCodigo { get; set; }

        [Column("Destino")]
        [MaxLength(10)]
        public string? DestinoCodigo { get; set; }

        [Column("Matricula")]
        [MaxLength(15)]
        public string? Matricula { get; set; }

        [Column("Estado")]
        [MaxLength(20)]
        public string? Estado { get; set; } = "Programado";

        [Column("TipoVuelo")]
        [MaxLength(15)]
        public string? TipoVuelo { get; set; } = "SoloIda"; // "SoloIda", "IdaYVuelta"

        [Column("FechaRegreso")]
        public DateTime? FechaRegreso { get; set; } // Solo para vuelos de ida y vuelta

        [ForeignKey(nameof(OrigenCodigo))]
        [InverseProperty(nameof(Aeropuerto.VuelosOrigen))]
        public Aeropuerto? Origen { get; set; }

        [ForeignKey(nameof(DestinoCodigo))]
        [InverseProperty(nameof(Aeropuerto.VuelosDestino))]
        public Aeropuerto? Destino { get; set; }

        [ForeignKey(nameof(Matricula))]
        public Aeronave? Aeronave { get; set; }

        [InverseProperty(nameof(Reserva.Vuelo))]
        public ICollection<Reserva> Reservas { get; set; }

        // Relación muchos-a-muchos con Tripulación
        public ICollection<Tripulacion> Tripulaciones { get; set; }

        public EstadoVuelo? EstadoVueloDetalle { get; set; }

        public decimal CalcularPrecioTotal(decimal precioBase, string clase)
        {
            decimal montoAdicional = clase switch
            {
                "Primera" => 200m,
                "Ejecutiva" => 100m,
                "Economica" => 0m,
                _ => 0m
            };

            return precioBase + montoAdicional;
        }
    }
}
