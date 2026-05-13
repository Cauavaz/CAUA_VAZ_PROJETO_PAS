using Microsoft.EntityFrameworkCore;
using Paschoalotto.Domain.Entities;

namespace Paschoalotto.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Produto> Produtos { get; set; }
    public DbSet<Titulo> Titulos { get; set; }
    public DbSet<Parcela> Parcelas { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(256);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.PasswordHash).IsRequired();
        });

        modelBuilder.Entity<Produto>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nome).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Preco).HasColumnType("decimal(18,2)");
        });

        modelBuilder.Entity<Titulo>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.NumeroTitulo).IsRequired().HasMaxLength(50);
            entity.HasIndex(e => e.NumeroTitulo).IsUnique();
            entity.Property(e => e.NomeDevedor).IsRequired().HasMaxLength(200);
            entity.Property(e => e.CpfDevedor).IsRequired().HasMaxLength(11);
            entity.Property(e => e.PercentualJuros).HasColumnType("decimal(9,4)");
            entity.Property(e => e.PercentualMulta).HasColumnType("decimal(9,4)");
            entity.Property(e => e.CriadoEm).IsRequired();

            entity.HasMany(t => t.Parcelas)
                  .WithOne(p => p.Titulo)
                  .HasForeignKey(p => p.TituloId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.Metadata
                  .FindNavigation(nameof(Titulo.Parcelas))!
                  .SetPropertyAccessMode(PropertyAccessMode.Field);
        });

        modelBuilder.Entity<Parcela>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Numero).IsRequired();
            entity.Property(e => e.DataVencimento).IsRequired();
            entity.Property(e => e.Valor).HasColumnType("decimal(18,2)").IsRequired();
        });
    }
}
