using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace AerolineaRD.Entity
{
    [Table("Cliente")]
    public class Cliente
    {
        public Cliente()
        {
            Reservas = new HashSet<Reserva>();
        }

        [Key]
        [Column("IdCliente")]
        public int Id { get; set; }

        [Column("Nombre")]
        [MaxLength(50)]
        public string? Nombre { get; set; }

        [Column("Email")]
        [MaxLength(50)]
        public string? Email { get; set; }

        [Column("Telefono")]
        [MaxLength(15)]
        public string? Telefono { get; set; }

        [Column("UserId")]
        [MaxLength(450)]
        public string? UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public IdentityUser? User { get; set; }

        [InverseProperty(nameof(Reserva.Cliente))]
        public ICollection<Reserva> Reservas { get; set; }
    }
}
