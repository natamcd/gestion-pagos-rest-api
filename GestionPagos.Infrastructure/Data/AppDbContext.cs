using GestionPagos.Core.Entities;
using GestionPagos.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GestionPagos.Infrastructure.Data;

public class AppDbContext : DbContext, IAppDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Usuario> Usuarios { get; set; } = null!;
    public DbSet<Transaccion> Transacciones { get; set; } = null!;
    public DbSet<Elemento> Elementos { get; set; } = null!;
    public DbSet<Naturaleza> Naturalezas { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Usuario configuration
        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.Property(e => e.FechaCreacion).IsRequired();
            entity.HasMany(e => e.Transacciones)
                  .WithOne(t => t.Usuario)
                  .HasForeignKey(t => t.UsuarioId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Elemento configuration
        modelBuilder.Entity<Elemento>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Descripcion).HasMaxLength(500);
            entity.HasMany(e => e.Transacciones)
                  .WithOne(t => t.Elemento)
                  .HasForeignKey(t => t.ElementoId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Naturaleza configuration
        modelBuilder.Entity<Naturaleza>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(50);
            entity.HasMany(e => e.Transacciones)
                  .WithOne(t => t.Naturaleza)
                  .HasForeignKey(t => t.NaturalezaId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Transaccion configuration
        modelBuilder.Entity<Transaccion>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Monto).IsRequired().HasColumnType("decimal(18,2)");
            entity.Property(e => e.Fecha).IsRequired();
            entity.Property(e => e.Descripcion).HasMaxLength(500);
        });
    }
}
