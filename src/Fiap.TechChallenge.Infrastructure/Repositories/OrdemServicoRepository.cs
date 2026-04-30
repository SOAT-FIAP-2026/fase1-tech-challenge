using Fiap.TechChallenge.Domain.Entities;
using Fiap.TechChallenge.Domain.Interfaces.Repository;
using Fiap.TechChallenge.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

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
            // If the aggregate was not loaded by this DbContext (detached), attach it.
            var entry = _context.Entry(ordemServico);

            if (entry.State == EntityState.Detached)
            {
                _context.OrdensServico.Attach(ordemServico);
            }

            // Ensure new child items are inserted. New Item entities are created with GUIDs,
            // so EF cannot rely on default key values to infer state. Detect items that do
            // not exist in the database and add them explicitly to the DbContext so they
            // produce INSERT statements instead of problematic UPDATEs.
            foreach (var item in ordemServico.ItensServico)
            {
                bool exists = await _context.ItensServico.AnyAsync(i => i.Id == item.Id);
                if (!exists)
                {
                    _context.ItensServico.Add(item);
                }
                else
                {
                    _context.Entry(item).State = EntityState.Unchanged;
                }
            }

            foreach (var item in ordemServico.ItensPecaInsumo)
            {
                bool exists = await _context.ItensPecaInsumo.AnyAsync(i => i.Id == item.Id);
                if (!exists)
                {
                    _context.ItensPecaInsumo.Add(item);
                }
                else
                {
                    _context.Entry(item).State = EntityState.Unchanged;
                }
            }

            // Also ensure Orcamento is added/updated appropriately.
            if (ordemServico.Orcamento != null)
            {
                bool orcExists = await _context.Orcamentos.AnyAsync(o => o.Id == ordemServico.Orcamento.Id);
                if (!orcExists)
                    _context.Orcamentos.Add(ordemServico.Orcamento);
                else
                    _context.Entry(ordemServico.Orcamento).State = EntityState.Modified;
            }

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
