using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AerolineaRD.Entity
{
    [Table("Factura")]
    public class Factura
    {
        [Key]
        [Column("CodFactura")]
        [MaxLength(10)]
        public string Codigo { get; set; } = null!;

        [Column("CodReserva")]
        [MaxLength(10)]
        public string? CodReserva { get; set; }

        [Column("Monto", TypeName = "decimal(10,2)")]
        public decimal Monto { get; set; }

        [Column("MetodoPago")]
        [MaxLength(20)]
        public string? MetodoPago { get; set; }

        [Column("FechaEmision")]
        public DateTime FechaEmision { get; set; } = DateTime.Now;

        [Column("EstadoPago")]
        [MaxLength(20)]
        public string? EstadoPago { get; set; } = "Pendiente"; // "Pendiente", "Pagado", "Reembolsado"

        [ForeignKey(nameof(CodReserva))]
        public Reserva? Reserva { get; set; }
    }
}
