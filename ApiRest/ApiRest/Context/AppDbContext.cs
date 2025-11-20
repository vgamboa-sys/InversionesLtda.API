using ApiRest.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiRest.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Region> Region { get; set; }
        public DbSet<Provincia> Provincia { get; set; }
        public DbSet<Comuna> Comuna { get; set; }
        
        public DbSet<Producto> Producto { get; set; }
        public DbSet<Stock> Stock { get; set; }
        public DbSet<Sucursal> Sucursal { get; set; }
        public DbSet<Tarjeta> Tarjeta { get; set; }
        


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configuración de Region
            modelBuilder.Entity<Region>()
                .Property(r => r.CodRegion)
                .ValueGeneratedNever();
            modelBuilder.Entity<Region>()
                .HasKey(r => r.CodRegion);

            // Configuración de Provincia
            modelBuilder.Entity<Provincia>()
                .Property(p => p.CodProvincia)
                .ValueGeneratedNever();
            modelBuilder.Entity<Provincia>()
                .HasKey(p => new { p.CodRegion, p.CodProvincia });

            modelBuilder.Entity<Provincia>()
                .HasOne(p => p.Region)
                .WithMany(r => r.Provincia)
                .HasForeignKey(p => p.CodRegion)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuración de Comuna
            modelBuilder.Entity<Comuna>()
                .Property(c => c.CodComuna)
                .ValueGeneratedNever();
            modelBuilder.Entity<Comuna>()
                .HasKey(c => new { c.CodRegion, c.CodProvincia, c.CodComuna });

            modelBuilder.Entity<Comuna>()
                .HasOne(c => c.Provincia)
                .WithMany(p => p.Comuna)
                .HasForeignKey(c => new { c.CodRegion, c.CodProvincia })
                .OnDelete(DeleteBehavior.Restrict);

    

            // Configuración de Sucursal
            modelBuilder.Entity<Sucursal>()
                .Property(s => s.CodSucursal)
                .ValueGeneratedNever();
            modelBuilder.Entity<Sucursal>()
                .HasKey(s => s.CodSucursal);

            modelBuilder.Entity<Sucursal>()
                .HasOne(s => s.Comuna)
                .WithMany(c => c.Sucursal)
                .HasForeignKey(s => new { s.CodRegion, s.CodProvincia, s.CodComuna })
                .OnDelete(DeleteBehavior.Restrict);

            // Configuración de Producto
            modelBuilder.Entity<Producto>()
                .Property(p => p.CodProducto)
                .ValueGeneratedNever();
            modelBuilder.Entity<Producto>()
                .HasKey(p => p.CodProducto);

            // Configuración de Stock
            modelBuilder.Entity<Stock>()
                .Property(st => st.CodStock)
                .ValueGeneratedNever();
            modelBuilder.Entity<Stock>()
                .HasKey(st => st.CodStock);

            modelBuilder.Entity<Stock>()
                .HasOne(st => st.Producto)
                .WithMany(p => p.Stock)
                .HasForeignKey(st => st.CodProducto)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Stock>()
                .HasOne(st => st.Sucursal)
                .WithMany(s => s.Stock)
                .HasForeignKey(st => st.CodSucursal)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuración de Tarjeta
            modelBuilder.Entity<Tarjeta>()
                .HasKey(t => t.CodTransaccion); // Definir la clave primaria

            modelBuilder.Entity<Tarjeta>()
                .Property(t => t.CodTransaccion)
                .IsRequired()
                .HasMaxLength(50) // Ajusta el tamaño según la longitud de BuyOrder
                .ValueGeneratedNever(); // Evita que EF intente generarlo dtomáticamente



            base.OnModelCreating(modelBuilder);
        }
    }
}