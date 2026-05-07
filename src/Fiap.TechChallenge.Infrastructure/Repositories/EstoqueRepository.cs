using Fiap.TechChallenge.Domain.Entities;
using Fiap.TechChallenge.Domain.Interfaces.Repository;
using Fiap.TechChallenge.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Fiap.TechChallenge.Infrastructure.Repositories
{
    public class EstoqueRepository(ApplicationDbContext context) : IEstoqueRepository
    {
        private readonly ApplicationDbContext _context = context;

        public async Task<int?> VerificarQuantidadePorIdPecaInsumo(Guid idPecaInsumo)
        {
            var estoque = await _context.Estoques
                .FirstOrDefaultAsync(e => e.IdPecaInsumo == idPecaInsumo);
            
            return estoque?.Quantidade;
        }

        public async Task<int?> VerificarQuantidadePorDescricaoPeca(string descricao)
        {
            if (string.IsNullOrWhiteSpace(descricao))
                return null;

            string termo = descricao.Trim();

            var estoques = await _context.Estoques
                .AsNoTracking()
                .Include(e => e.PecaInsumo)
                .ToListAsync();

            Estoque? estoque = estoques
                .FirstOrDefault(e => e.PecaInsumo.Descricao.Contains(termo));

            return estoque?.Quantidade;
        }

        public async Task<Estoque?> ObterPorIdPecaInsumo(Guid idPecaInsumo)
        {
            return await _context.Estoques
                .FirstOrDefaultAsync(e => e.IdPecaInsumo == idPecaInsumo);
        }

        public async Task Adicionar(Estoque estoque)
        {
            await _context.Estoques.AddAsync(estoque);
            await _context.SaveChangesAsync();
        }

        public async Task Atualizar(Estoque estoque)
        {
            _context.Estoques.Update(estoque);
            await _context.SaveChangesAsync();
        }

        public async Task Deletar(Guid idEstoque)
        {
            var estoque = await _context.Estoques.FirstOrDefaultAsync(e => e.Id == idEstoque);
            
            if (estoque != null)
            {
                estoque.MarcarComoApagado();
                _context.Estoques.Update(estoque);
                await _context.SaveChangesAsync();
            }
        }
    }
}
