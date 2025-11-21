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
        
        // Nuevas tablas
        public DbSet<Aeronave> Aeronaves { get; set; }
        public DbSet<Tripulacion> Tripulaciones { get; set; }
        public DbSet<VueloTripulacion> VueloTripulaciones { get; set; }
        public DbSet<EstadoVuelo> EstadosVuelo { get; set; }
        public DbSet<Notificacion> Notificaciones { get; set; }
        public DbSet<TicketSoporte> TicketsSoporte { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Vuelo>()
                .HasMany(v => v.Tripulaciones)
                .WithMany(t => t.Vuelos)
                .UsingEntity<VueloTripulacion>(
                    j => j
                        .HasOne(vt => vt.Tripulacion)
                        .WithMany()
                        .HasForeignKey(vt => vt.IdTripulacion),
                    j => j
                        .HasOne(vt => vt.Vuelo)
                        .WithMany()
                        .HasForeignKey(vt => vt.IdVuelo),
                    j =>
                    {
                        j.HasKey(vt => vt.Id);
                        j.ToTable("VueloTripulacion");
                    });

            // Relación uno a uno entre Vuelo y EstadoVuelo
            modelBuilder.Entity<EstadoVuelo>()
                .HasOne(e => e.Vuelo)
                .WithOne(v => v.EstadoVueloDetalle)
                .HasForeignKey<EstadoVuelo>(e => e.IdVuelo);
        }
    }
}
