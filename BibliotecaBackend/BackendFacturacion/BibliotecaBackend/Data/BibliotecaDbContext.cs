using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaBackend.Data;

public partial class BibliotecaDbContext : DbContext
{
    public BibliotecaDbContext()
    {
    }

    public BibliotecaDbContext(DbContextOptions<BibliotecaDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Articulo> Articulos { get; set; }

    public virtual DbSet<AsientoContable> AsientoContables { get; set; }

    public virtual DbSet<CiudadEntrega> CiudadEntregas { get; set; }

    public virtual DbSet<Cliente> Clientes { get; set; }

    public virtual DbSet<Empleado> Empleados { get; set; }

    public virtual DbSet<FacturaCabecera> FacturaCabeceras { get; set; }

    public virtual DbSet<FacturaDetalle> FacturaDetalles { get; set; }

    public virtual DbSet<MotivoNomina> MotivoNominas { get; set; }

    public virtual DbSet<NominaCabecera> NominaCabeceras { get; set; }

    public virtual DbSet<NominaDetalle> NominaDetalles { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Data Source=localhost;Initial Catalog=Biblioteca;Integrated Security=True;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Articulo>(entity =>
        {
            entity.HasKey(e => e.CodigoArticulo).HasName("PK__Articulo__5A774984D784A8C7");

            entity.ToTable("Articulo");

            entity.Property(e => e.NombreArticulo).HasMaxLength(100);
        });

        modelBuilder.Entity<AsientoContable>(entity =>
        {
            entity.HasKey(e => e.IdAsientoContable).HasName("PK__AsientoC__1909203DE986A91F");

            entity.ToTable("AsientoContable");

            entity.Property(e => e.IdAsientoContable).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CuentaCredito).HasMaxLength(50);
            entity.Property(e => e.CuentaDebito).HasMaxLength(50);
            entity.Property(e => e.Monto).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TipoOperacion).HasMaxLength(50);
        });

        modelBuilder.Entity<CiudadEntrega>(entity =>
        {
            entity.HasKey(e => e.CodigoCiudad).HasName("PK__CiudadEn__F1EB6A3AFA30EC8A");

            entity.ToTable("CiudadEntrega");

            entity.Property(e => e.NombreCiudad).HasMaxLength(100);
        });

        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.HasKey(e => e.Ruc).HasName("PK__Cliente__CAF3326AAC498161");

            entity.ToTable("Cliente");

            entity.Property(e => e.Ruc)
                .HasMaxLength(13)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("RUC");
            entity.Property(e => e.Direccion).HasMaxLength(200);
            entity.Property(e => e.Nombre).HasMaxLength(100);
        });

        modelBuilder.Entity<Empleado>(entity =>
        {
            entity.HasKey(e => e.Cedula).HasName("PK__Empleado__B4ADFE398579CF47");

            entity.ToTable("Empleado");

            entity.Property(e => e.Cedula)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Nombre).HasMaxLength(100);
            entity.Property(e => e.Sueldo).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<FacturaCabecera>(entity =>
        {
            entity.HasKey(e => e.NumeroFactura).HasName("PK__FacturaC__CF12F9A7CA6A9328");

            entity.ToTable("FacturaCabecera");

            entity.Property(e => e.Ruccliente)
                .HasMaxLength(13)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("RUCCliente");

            entity.HasOne(d => d.CodigoCiudadEntregaNavigation).WithMany(p => p.FacturaCabeceras)
                .HasForeignKey(d => d.CodigoCiudadEntrega)
                .HasConstraintName("FK_FacturaCab_Ciudad");

            entity.HasOne(d => d.RucclienteNavigation).WithMany(p => p.FacturaCabeceras)
                .HasForeignKey(d => d.Ruccliente)
                .HasConstraintName("FK_FacturaCab_Cliente");
        });

        modelBuilder.Entity<FacturaDetalle>(entity =>
        {
            entity.HasKey(e => e.IdFacturaDetalle).HasName("PK__FacturaD__3D8E1AB810441947");

            entity.ToTable("FacturaDetalle", tb =>
                {
                    tb.HasTrigger("trg_AsientoFactura");
                    tb.HasTrigger("trg_ValidarInventario");
                });

            entity.Property(e => e.Precio).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.CodigoArticuloNavigation).WithMany(p => p.FacturaDetalles)
                .HasForeignKey(d => d.CodigoArticulo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FacturaDet_Articulo");

            entity.HasOne(d => d.NumeroFacturaNavigation).WithMany(p => p.FacturaDetalles)
                .HasForeignKey(d => d.NumeroFactura)
                .HasConstraintName("FK_FacturaDet_Cabecera");
        });

        modelBuilder.Entity<MotivoNomina>(entity =>
        {
            entity.HasKey(e => e.CodigoMotivo).HasName("PK__MotivoNo__6AAC1B9982E81863");

            entity.ToTable("MotivoNomina");

            entity.Property(e => e.NombreMotivo).HasMaxLength(100);
            entity.Property(e => e.Tipo)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();
        });

        modelBuilder.Entity<NominaCabecera>(entity =>
        {
            entity.HasKey(e => e.NumeroNomina).HasName("PK__NominaCa__E8DAB6E37B9841DA");

            entity.ToTable("NominaCabecera");

            entity.Property(e => e.CedulaEmpleado)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength();

            entity.HasOne(d => d.CedulaEmpleadoNavigation).WithMany(p => p.NominaCabeceras)
                .HasForeignKey(d => d.CedulaEmpleado)
                .HasConstraintName("FK_NominaCab_Empleado");
        });

        modelBuilder.Entity<NominaDetalle>(entity =>
        {
            entity.HasKey(e => e.IdNominaDetalle).HasName("PK__NominaDe__C872BFDD1FBA2696");

            entity.ToTable("NominaDetalle", tb => tb.HasTrigger("trg_AsientoNomina"));

            entity.Property(e => e.Valor).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.CodigoMotivoNavigation).WithMany(p => p.NominaDetalles)
                .HasForeignKey(d => d.CodigoMotivo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_NominaDet_Motivo");

            entity.HasOne(d => d.NumeroNominaNavigation).WithMany(p => p.NominaDetalles)
                .HasForeignKey(d => d.NumeroNomina)
                .HasConstraintName("FK_NominaDet_Cabecera");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Usuario__3214EC076208CCDF");

            entity.ToTable("Usuario");

            entity.Property(e => e.Correo).HasMaxLength(100);
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.NombreUsuario).HasMaxLength(50);
            entity.Property(e => e.Rol).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
