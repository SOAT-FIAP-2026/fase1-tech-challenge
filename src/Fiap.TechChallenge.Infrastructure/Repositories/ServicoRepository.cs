using Fiap.TechChallenge.Domain.Entities;
using Fiap.TechChallenge.Domain.Interfaces.Repository;
using Fiap.TechChallenge.Domain.ValueObjects;
using Fiap.TechChallenge.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Fiap.TechChallenge.Infrastructure.Repositories
{
    public class ServicoRepository(ApplicationDbContext context) : IServicoRepository
    {
        private readonly ApplicationDbContext _context = context;

        public async Task<Servico?> ObterPorId(Guid id)
        {
            return await _context.Servicos.FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<IReadOnlyCollection<Servico>> ObterTodos()
        {
            return await _context.Servicos
                .OrderBy(s => s.Nome)
                .ToListAsync();
        }

        public async Task<IReadOnlyCollection<Servico>> ObterPorIds(IReadOnlyCollection<Guid> ids)
        {
            Guid[] idsDistinctos = [.. ids.Distinct()];

            return await _context.Servicos
                .Where(s => idsDistinctos.Contains(s.Id))
                .ToListAsync();
        }

        public async Task<bool> ExisteNome(string nome, Guid? ignorarId = null)
        {
            NomeVO nomeVO = new(nome);

            return await _context.Servicos.AnyAsync(s =>
                s.Nome == nomeVO &&
                (!ignorarId.HasValue || s.Id != ignorarId.Value)
            );
        }

        public async Task Adicionar(Servico servico)
        {
            await _context.Servicos.AddAsync(servico);
            await _context.SaveChangesAsync();
        }

        public async Task Atualizar(Servico servico)
        {
            _context.Servicos.Update(servico);
            await _context.SaveChangesAsync();
        }

        public async Task Deletar(Servico servico)
        {
            servico.MarcarComoApagado();
            _context.Servicos.Update(servico);
            await _context.SaveChangesAsync();
        }
    }
}
