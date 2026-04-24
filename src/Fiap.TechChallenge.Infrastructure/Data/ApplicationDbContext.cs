using Fiap.TechChallenge.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Fiap.TechChallenge.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Permissao> Permissoes => Set<Permissao>();
        public DbSet<Usuario> Usuarios => Set<Usuario>();
        public DbSet<Cliente> Clientes => Set<Cliente>();
        public DbSet<StatusOrdemServico> StatusOrdensServico => Set<StatusOrdemServico>();
        public DbSet<Veiculo> Veiculos => Set<Veiculo>();
        public DbSet<Servico> Servicos => Set<Servico>();
        public DbSet<PecaInsumo> PecasInsumo => Set<PecaInsumo>();
        public DbSet<OrdemServico> OrdensServico => Set<OrdemServico>();
        public DbSet<Estoque> Estoques => Set<Estoque>();
        public DbSet<Orcamento> Orcamentos => Set<Orcamento>();
        public DbSet<ItemServico> ItensServico => Set<ItemServico>();
        public DbSet<ItemPecaInsumo> ItensPecaInsumo => Set<ItemPecaInsumo>();

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }
    }
}
