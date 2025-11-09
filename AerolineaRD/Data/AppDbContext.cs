using AerolineaRD.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AerolineaRD.Data
{
    public class AppDbContext : IdentityDbContext<IdentityUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Tablas del dominio
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Pasajero> Pasajeros { get; set; }
        public DbSet<Vuelo> Vuelos { get; set; }
        public DbSet<Aeropuerto> Aeropuertos { get; set; }
        public DbSet<Reserva> Reservas { get; set; }
        public DbSet<Factura> Facturas { get; set; }
        public DbSet<Equipaje> Equipajes { get; set; }
        public DbSet<Asiento> Asientos { get; set; }
    }
}
