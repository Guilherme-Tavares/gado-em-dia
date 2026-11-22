using GadoEmDia.Models;
using Microsoft.EntityFrameworkCore;

namespace GadoEmDia.Data;

public class GadoContext : DbContext
{
    public GadoContext(DbContextOptions<GadoContext> options)
        : base(options)
    {
    }

    public DbSet<Bovino> Bovinos => Set<Bovino>();
    public DbSet<Produto> Produtos => Set<Produto>();
    public DbSet<Aplicador> Aplicadores => Set<Aplicador>();
    public DbSet<Aplicacao> Aplicacoes => Set<Aplicacao>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Bovino>(bovino =>
        {
            bovino.HasIndex(b => b.Identificacao).IsUnique();
            bovino.Property(b => b.DataNascimento).HasColumnType("date");
            bovino.Property(b => b.PesoKg).HasPrecision(6, 2);
        });

        modelBuilder.Entity<Aplicacao>(aplicacao =>
        {
            aplicacao.Property(a => a.Data).HasColumnType("date");

            aplicacao.HasOne(a => a.Bovino)
                .WithMany(b => b.Aplicacoes)
                .HasForeignKey(a => a.BovinoId)
                .OnDelete(DeleteBehavior.Restrict);

            aplicacao.HasOne(a => a.Aplicador)
                .WithMany(ap => ap.Aplicacoes)
                .HasForeignKey(a => a.AplicadorId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<AplicacaoProduto>(item =>
        {
            item.HasKey(i => new { i.AplicacaoId, i.ProdutoId });
            item.Property(i => i.Dose).HasPrecision(7, 2);

            item.HasOne(i => i.Aplicacao)
                .WithMany(a => a.Itens)
                .HasForeignKey(i => i.AplicacaoId)
                .OnDelete(DeleteBehavior.Cascade);

            item.HasOne(i => i.Produto)
                .WithMany(p => p.Itens)
                .HasForeignKey(i => i.ProdutoId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
