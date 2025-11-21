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

        // 🔹 Nuevos DbSet para usuarios, roles y ventas
        public DbSet<Usuario> Usuario { get; set; }
        public DbSet<Rol> Rol { get; set; }
        public DbSet<UsuarioRol> UsuarioRol { get; set; }

        public DbSet<Cliente> Cliente { get; set; }
        public DbSet<Boleta> Boleta { get; set; }
        public DbSet<DetalleBoleta> DetalleBoleta { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ============================
            // Configuración de Region
            // ============================
            modelBuilder.Entity<Region>()
                .Property(r => r.CodRegion)
                .ValueGeneratedNever();
            modelBuilder.Entity<Region>()
                .HasKey(r => r.CodRegion);

            // ============================
            // Configuración de Provincia
            // ============================
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

            // ============================
            // Configuración de Comuna
            // ============================
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

            // ============================
            // Configuración de Sucursal
            // ============================
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

            // ============================
            // Configuración de Producto
            // ============================
            modelBuilder.Entity<Producto>()
                .Property(p => p.CodProducto)
                .ValueGeneratedNever();
            modelBuilder.Entity<Producto>()
                .HasKey(p => p.CodProducto);

            // ============================
            // Configuración de Stock
            // ============================
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

            // ============================
            // Configuración de Tarjeta
            // ============================
            modelBuilder.Entity<Tarjeta>()
                .HasKey(t => t.CodTransaccion); // Definir la clave primaria

            modelBuilder.Entity<Tarjeta>()
                .Property(t => t.CodTransaccion)
                .IsRequired()
                .HasMaxLength(50) // Ajusta el tamaño según la longitud de BuyOrder
                .ValueGeneratedNever(); // Evita que EF intente generarlo automáticamente
                                        // 👇 NUEVO (opcional, pero ordenado)

            modelBuilder.Entity<Tarjeta>()
                .Property(t => t.Token)
                .HasMaxLength(255);

            // ===========================================================
            // 🔹 NUEVOS MODELOS
            // ===========================================================

            // ============================
            // Usuario
            // ============================
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasKey(u => u.Id);

                entity.Property(u => u.Username)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(u => u.PasswordHash)
                    .IsRequired();

                entity.Property(u => u.Email)
                    .HasMaxLength(200);
            });

            // ============================
            // Rol
            // ============================
            modelBuilder.Entity<Rol>(entity =>
            {
                entity.HasKey(r => r.Id);

                entity.Property(r => r.Nombre)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            // ============================
            // UsuarioRol (muchos-a-muchos)
            // ============================
            modelBuilder.Entity<UsuarioRol>(entity =>
            {
                // Clave compuesta
                entity.HasKey(ur => new { ur.UsuarioId, ur.RolId });

                entity.HasOne(ur => ur.Usuario)
                    .WithMany(u => u.UsuarioRoles)
                    .HasForeignKey(ur => ur.UsuarioId);

                entity.HasOne(ur => ur.Rol)
                    .WithMany(r => r.UsuarioRoles)
                    .HasForeignKey(ur => ur.RolId);
            });

            // ============================
            // Cliente
            // ============================
            modelBuilder.Entity<Cliente>(entity =>
            {
                entity.HasKey(c => c.NumRun); // PK = RUN numérico

                entity.Property(c => c.NumRun)
                    .ValueGeneratedNever();

                entity.Property(c => c.DvRun)
                    .IsRequired()
                    .HasMaxLength(1);

                entity.Property(c => c.P_Nombre)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(c => c.S_Nombre)
                    .HasMaxLength(100);

                entity.Property(c => c.A_Paterno)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(c => c.A_Materno)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(c => c.Correo)
                    .HasMaxLength(200);

                entity.Property(c => c.Direccion)
                    .HasMaxLength(300);

                // Relación 1–1 opcional con Usuario
                entity.HasOne(c => c.Usuario)
                    .WithOne(u => u.Cliente)
                    .HasForeignKey<Cliente>(c => c.UsuarioId)
                    .OnDelete(DeleteBehavior.SetNull);

                // Relación con Región
                entity.HasOne(c => c.Region)
                    .WithMany()
                    .HasForeignKey(c => c.CodRegion);

                // Relación con Provincia (clave compuesta)
                entity.HasOne(c => c.Provincia)
                    .WithMany()
                    .HasForeignKey(c => new { c.CodRegion, c.CodProvincia })
                    .OnDelete(DeleteBehavior.Restrict);

                // Relación con Comuna (clave compuesta)
                entity.HasOne(c => c.Comuna)
                    .WithMany()
                    .HasForeignKey(c => new { c.CodRegion, c.CodProvincia, c.CodComuna })
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ============================
            // Boleta
            // ============================
            modelBuilder.Entity<Boleta>(entity =>
            {
                entity.HasKey(b => b.CodBoleta);

                entity.Property(b => b.CorreoContacto)
                    .HasMaxLength(200);

                // Relación Boleta–Cliente
                entity.HasOne(b => b.Cliente)
                    .WithMany(c => c.Boletas)
                    .HasForeignKey(b => b.RunCliente)
                    .OnDelete(DeleteBehavior.Restrict);

                // Relación Boleta–Tarjeta (por CodTransaccion)
                entity.HasOne(b => b.Tarjeta)
                    .WithMany() // si quieres, puedes luego añadir ICollection<Boleta> en Tarjeta
                    .HasForeignKey(b => b.CodTransaccion)
                    .HasPrincipalKey(t => t.CodTransaccion);
            });

            // ============================
            // DetalleBoleta
            // ============================
            modelBuilder.Entity<DetalleBoleta>(entity =>
            {
                entity.HasKey(d => d.Id);

                // Relación Detalle–Boleta
                entity.HasOne(d => d.Boleta)
                    .WithMany(b => b.Detalles)
                    .HasForeignKey(d => d.CodBoleta)
                    .OnDelete(DeleteBehavior.Cascade);

                // Relación Detalle–Producto
                entity.HasOne(d => d.Producto)
                    .WithMany() // si quieres podrías agregar ICollection<DetalleBoleta> en Producto
                    .HasForeignKey(d => d.CodProducto);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
