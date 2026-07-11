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
//@todo  revisar
        public async Task Atualizar(OrdemServico ordemServico)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            bool autoDetectChangesOriginal = _context.ChangeTracker.AutoDetectChangesEnabled;
            _context.ChangeTracker.AutoDetectChangesEnabled = false;
            try
            {
                // If the aggregate was not loaded by this DbContext (detached), attach it.
                if (!_context.OrdensServico.Local.Any(o => o.Id == ordemServico.Id))
                {
                    _context.OrdensServico.Attach(ordemServico);
                }

                // Remove child entities that were removed from the aggregate collections.
                // This must happen before any query (AnyAsync), because EF will run change
                // detection and fail on required relationships if dependents are only severed.
                foreach (EntityEntry<ItemServico> trackedItem in _context.ChangeTracker.Entries<ItemServico>()
                             .Where(e => e.Entity.IdOrdemServico == ordemServico.Id)
                             .Where(e => !ordemServico.ItensServico.Any(item => item.Id == e.Entity.Id))
                             .ToList())
                {
                    trackedItem.State = EntityState.Deleted;
                }

                foreach (EntityEntry<ItemPecaInsumo> trackedItem in _context.ChangeTracker.Entries<ItemPecaInsumo>()
                             .Where(e => e.Entity.IdOrdemServico == ordemServico.Id)
                             .Where(e => !ordemServico.ItensPecaInsumo.Any(item => item.Id == e.Entity.Id))
                             .ToList())
                {
                    trackedItem.State = EntityState.Deleted;
                }
            }
            finally
            {
                _context.ChangeTracker.AutoDetectChangesEnabled = autoDetectChangesOriginal;
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
                    _context.Entry(item).State = EntityState.Modified;
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
            await transaction.CommitAsync();
        }

        public async Task<OrdemServico?> ObterPorId(Guid id)
        {
            return await _context.OrdensServico
                .Include(o => o.Cliente)
                .Include(o => o.Veiculo)
                .Include(o => o.Status)
                .Include(o => o.Orcamento)
                .Include(o => o.ItensServico)
                    .ThenInclude(item => item.Servico)
                .Include(o => o.ItensPecaInsumo)
                    .ThenInclude(item => item.PecaInsumo)
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
                    .ThenInclude(item => item.Servico)
                .Include(o => o.ItensPecaInsumo)
                    .ThenInclude(item => item.PecaInsumo)
                .OrderByDescending(o => o.DataAbertura)
                .ToListAsync();
        }

        public async Task<IReadOnlyCollection<ItemServico>> ObterItensServicoFinalizados()
        {
            return await _context.ItensServico
                .AsNoTracking()
                .Include(item => item.Servico)
                .Where(item => item.DataHoraInicio != null && item.DataHoraFim != null)
                .ToListAsync();
        }
    }
}
