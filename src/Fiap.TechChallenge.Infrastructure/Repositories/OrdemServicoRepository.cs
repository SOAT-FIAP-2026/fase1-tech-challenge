using Fiap.TechChallenge.Domain.Entities;
using Fiap.TechChallenge.Domain.Interfaces.Repository;
using Fiap.TechChallenge.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Fiap.TechChallenge.Infrastructure.Repositories
{
    public class OrdemServicoRepository(ApplicationDbContext context) : IOrdemServicoRepository
    {
        private readonly ApplicationDbContext _context = context;

        public async Task Adicionar(OrdemServico ordemServico)
        {
            await _context.OrdensServico.AddAsync(ordemServico);
            await _context.SaveChangesAsync();
        }

        public async Task Atualizar(OrdemServico ordemServico)
        {
            _context.OrdensServico.Update(ordemServico);
            await _context.SaveChangesAsync();
        }

        public async Task<OrdemServico?> ObterPorId(Guid id)
        {
            return await _context.OrdensServico
                .Include(o => o.Cliente)
                .Include(o => o.Veiculo)
                .Include(o => o.Status)
                .Include(o => o.Orcamento)
                .Include(o => o.ItensServico)
                .Include(o => o.ItensPecaInsumo)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<IReadOnlyCollection<OrdemServico>> ObterTodos()
        {
            return await _context.OrdensServico
                .Include(o => o.Cliente)
                .Include(o => o.Veiculo)
                .Include(o => o.Status)
                .Include(o => o.Orcamento)
                .Include(o => o.ItensServico)
                .Include(o => o.ItensPecaInsumo)
                .OrderByDescending(o => o.DataAbertura)
                .ToListAsync();
        }
    }
}
