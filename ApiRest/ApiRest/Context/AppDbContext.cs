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
        public DbSet<Cliente> Cliente { get; set; }
        public DbSet<Producto> Producto { get; set; }
        public DbSet<Stock> Stock { get; set; }
        public DbSet<Sucursal> Sucursal { get; set; }
        public DbSet<Tarjeta> Tarjeta { get; set; }
        public DbSet<Boleta> Boleta { get; set; }
        public DbSet<DetalleBoleta> DetalleBoleta { get; set; }


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

            // Configuración de Cliente
            modelBuilder.Entity<Cliente>()
                .Property(cl => cl.NumRun)
                .ValueGeneratedNever();
            modelBuilder.Entity<Cliente>()
                .HasKey(cl => cl.NumRun);

            modelBuilder.Entity<Cliente>()
                .HasOne(cl => cl.Region)
                .WithMany(r => r.Cliente)
                .HasForeignKey(cl => cl.CodRegion)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Cliente>()
                .HasOne(cl => cl.Provincia)
                .WithMany(p => p.Cliente)
                .HasForeignKey(cl => new { cl.CodRegion, cl.CodProvincia })
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Cliente>()
                .HasOne(cl => cl.Comuna)
                .WithMany(c => c.Cliente)
                .HasForeignKey(cl => new { cl.CodRegion, cl.CodProvincia, cl.CodComuna })
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
                .ValueGeneratedNever(); // Evita que EF intente generarlo automáticamente

            modelBuilder.Entity<Tarjeta>()
                .HasMany(t => t.Boleta)
                .WithOne(b => b.Tarjeta)
                .HasForeignKey(b => b.CodTransaccion)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuración de Boleta
            modelBuilder.Entity<Boleta>()
                .Property(b => b.CodBoleta)
                .ValueGeneratedNever();
            modelBuilder.Entity<Boleta>()
                .HasKey(b => b.CodBoleta);
            modelBuilder.Entity<Boleta>()
                .Property(b => b.RunCliente)
                .HasMaxLength(12) // Tamaño típico para un RUN con puntos y guion
                .IsRequired(); // Define el campo como obligatorio
            modelBuilder.Entity<Boleta>()
                .Property(b => b.Total)
                .IsRequired(); // Define el campo como obligatorio


            // Configuración de DetalleBoleta
            modelBuilder.Entity<Boleta>()
                .Property(b => b.RunCliente)
                .HasMaxLength(12)
                .IsRequired();

            modelBuilder.Entity<Boleta>()
                .Property(b => b.Total)
                .IsRequired();


            modelBuilder.Entity<DetalleBoleta>()
                .Property(db => db.CodDetalle)
                .ValueGeneratedNever();
            modelBuilder.Entity<DetalleBoleta>()
                .HasKey(db => db.CodDetalle);

            modelBuilder.Entity<DetalleBoleta>()
                .HasOne(db => db.Boleta)
                .WithMany(b => b.DetalleBoleta)
                .HasForeignKey(db => db.CodBoleta)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DetalleBoleta>()
                .HasOne(db => db.Producto)
                .WithMany(p => p.DetalleBoleta)
                .HasForeignKey(db => db.CodProducto)
                .OnDelete(DeleteBehavior.Restrict);



            base.OnModelCreating(modelBuilder);
        }
    }
}
