using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AerolineaRD.Entity
{
    [Table("TicketSoporte")]
    public class TicketSoporte
    {
        [Key]
        [Column("IdTicket")]
        public int Id { get; set; }

        [Column("IdCliente")]
        public int IdCliente { get; set; }

        [Column("Asunto")]
        [MaxLength(100)]
        public string? Asunto { get; set; }

        [Column("Descripcion")]
        [MaxLength(1000)]
        public string? Descripcion { get; set; }

        [Column("Estado")]
        [MaxLength(20)]
        public string? Estado { get; set; } = "Abierto"; // "Abierto", "En Proceso", "Resuelto", "Cerrado"

        [Column("Prioridad")]
        [MaxLength(20)]
        public string? Prioridad { get; set; } = "Media"; // "Baja", "Media", "Alta"

        [Column("FechaCreacion")]
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        [Column("FechaCierre")]
        public DateTime? FechaCierre { get; set; }

        [ForeignKey(nameof(IdCliente))]
        public Cliente? Cliente { get; set; }
    }
}