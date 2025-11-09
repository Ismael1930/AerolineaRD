using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AerolineaRD.Entity
{
    [Table("Reserva")]
    public class Reserva
    {
        [Key]
        [Column("CodReserva")]
        [MaxLength(10)]
        public string Codigo { get; set; } = null!;

        [Column("IdPasajero")]
        public int IdPasajero { get; set; }

        [Column("IdVuelo")]
        public int IdVuelo { get; set; }

        [Column("IdCliente")]
        public int IdCliente { get; set; }

        [Column("FechaReserva", TypeName = "date")]
        public DateTime FechaReserva { get; set; }

        [ForeignKey(nameof(IdPasajero))]
        public Pasajero? Pasajero { get; set; }

        [ForeignKey(nameof(IdVuelo))]
        public Vuelo? Vuelo { get; set; }

        [ForeignKey(nameof(IdCliente))]
        public Cliente? Cliente { get; set; }

        [InverseProperty(nameof(Factura.Reserva))]
        public Factura? Factura { get; set; }
    }
}
