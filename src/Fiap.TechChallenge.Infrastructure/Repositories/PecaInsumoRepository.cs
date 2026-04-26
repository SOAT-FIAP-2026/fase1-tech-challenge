using Fiap.TechChallenge.Domain.Entities;
using Fiap.TechChallenge.Domain.Interfaces.Repository;
using Fiap.TechChallenge.Domain.ValueObjects;
using Fiap.TechChallenge.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Fiap.TechChallenge.Infrastructure.Repositories
{
    public class PecaInsumoRepository : IPecaInsumoRepository
    {
        private readonly ApplicationDbContext _context;
        
        public PecaInsumoRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        
        public async Task<PecaInsumo> ObterPorId(Guid id)
        {
            return await _context.PecasInsumo.FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<PecaInsumo> ObterPorDescricao(string descricao) 
        {
            return await _context.PecasInsumo.FirstOrDefaultAsync(p => p.Descricao.Contains(descricao));
        }

        public async Task<IReadOnlyCollection<PecaInsumo>> ListarTodos()
        {
            return await _context.PecasInsumo.ToListAsync();
        }
        
        public async Task Adicionar(PecaInsumo pecaInsumo)
        {
            await _context.PecasInsumo.AddAsync(pecaInsumo);
            await _context.SaveChangesAsync();
        }

        public async Task Atualizar(PecaInsumo pecaInsumo)
        {
            _context.PecasInsumo.Update(pecaInsumo);
            await _context.SaveChangesAsync();
        }

        public async Task Deletar(PecaInsumo pecaInsumo)
        {
            pecaInsumo.MarcarComoApagado();
            _context.PecasInsumo.Update(pecaInsumo);
            await _context.SaveChangesAsync();
        }
    }
}
