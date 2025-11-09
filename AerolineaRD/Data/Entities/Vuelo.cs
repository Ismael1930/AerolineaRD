using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AerolineaRD.Entity
{
    [Table("Vuelo")]
    public class Vuelo
    {
        public Vuelo()
        {
            Asientos = new HashSet<Asiento>();
            Reservas = new HashSet<Reserva>();
        }

        [Key]
        [Column("IdVuelo")]
        public int Id { get; set; }

        [Column("NumeroVuelo")]
        [MaxLength(10)]
        public string? NumeroVuelo { get; set; }

        [Column("Fecha", TypeName = "date")]
        public DateTime Fecha { get; set; }

        [Column("Origen")]
        [MaxLength(10)]
        public string? OrigenCodigo { get; set; }

        [Column("Destino")]
        [MaxLength(10)]
        public string? DestinoCodigo { get; set; }

        [ForeignKey(nameof(OrigenCodigo))]
        [InverseProperty(nameof(Aeropuerto.VuelosOrigen))]
        public Aeropuerto? Origen { get; set; }

        [ForeignKey(nameof(DestinoCodigo))]
        [InverseProperty(nameof(Aeropuerto.VuelosDestino))]
        public Aeropuerto? Destino { get; set; }

        [InverseProperty(nameof(Asiento.Vuelo))]
        public ICollection<Asiento> Asientos { get; set; }

        [InverseProperty(nameof(Reserva.Vuelo))]
        public ICollection<Reserva> Reservas { get; set; }
    }
}
