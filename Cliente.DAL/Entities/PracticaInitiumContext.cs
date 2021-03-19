using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace PCliente.DAL.Entities
{
    public partial class DbContextCliente : DbContext
    {
        public DbContextCliente()
        {
        }

        public DbContextCliente(DbContextOptions<DbContextCliente> options)
            : base(options)
        {
        }

        public virtual DbSet<Cliente> Clientes { get; set; }
        public virtual DbSet<ClienteCola> ClienteColas { get; set; }
        public virtual DbSet<Cola> Colas { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Data Source=DESKTOP-BHROMAP;Initial Catalog=PracticaInitium;User ID=sa;Password=sadikson");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Modern_Spanish_CI_AS");

            modelBuilder.Entity<Cliente>(entity =>
            {
                entity.HasKey(e => e.IdCliente);

                entity.ToTable("Cliente");

                entity.Property(e => e.IdCliente)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("idCliente");

                entity.Property(e => e.AsignadoCola)
                    .IsRequired()
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('N')")
                    .HasComment("Tiene dos valores S y N");

                entity.Property(e => e.NombreCliente)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("nombreCliente");
            });

            modelBuilder.Entity<ClienteCola>(entity =>
            {
                entity.HasKey(e => new { e.IdCliente, e.IdCola })
                    .HasName("PK_ClienteCola")
                    .IsClustered(false);

                entity.ToTable("Cliente_Cola");

                entity.HasComment("CE= Cliente en Espera\r\nCA= Cliente Atendido\r\nCV= Cliente Ventanilla");

                entity.HasIndex(e => e.IdCliente, "IX_Cliente_Cola");

                entity.Property(e => e.IdCliente)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("idCliente");

                entity.Property(e => e.IdCola).HasColumnName("idCola");

                entity.Property(e => e.EstadoCliente)
                    .IsRequired()
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .HasComment("EstadoCliente valores CE Cliente en espera, CV Cliente en Ventanilla\r\nCP Cliente Procesado");

                entity.Property(e => e.TiempoFin).HasColumnType("datetime");

                entity.Property(e => e.TiempoInicio).HasColumnType("datetime");
            });

            modelBuilder.Entity<Cola>(entity =>
            {
                entity.HasKey(e => e.IdCola);

                entity.ToTable("Cola");

                entity.Property(e => e.IdCola).HasColumnName("idCola");

                entity.Property(e => e.EstadoCola)
                    .IsRequired()
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("estadoCola")
                    .HasComment("tiene dos valores O= Ocupado\r\nL= Libre");

                entity.Property(e => e.NombreCola)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("nombreCola");

                entity.Property(e => e.TiempoCola).HasColumnName("tiempoCola");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
