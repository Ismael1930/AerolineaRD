using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AerolineaRD.Entity
{
    [Table("Notificacion")]
    public class Notificacion
    {
        [Key]
        [Column("IdNotificacion")]
        public int Id { get; set; }

        [Column("IdCliente")]
        public int IdCliente { get; set; }

        [Column("Tipo")]
        [MaxLength(30)]
        public string? Tipo { get; set; } // "Confirmacion", "Cambio", "Retraso", "Cancelacion", "Promocion"

        [Column("Mensaje")]
        [MaxLength(500)]
        public string? Mensaje { get; set; }

        [Column("FechaEnvio")]
        public DateTime FechaEnvio { get; set; } = DateTime.Now;

        [Column("Leida")]
        public bool Leida { get; set; } = false;

        [ForeignKey(nameof(IdCliente))]
        public Cliente? Cliente { get; set; }
    }
}