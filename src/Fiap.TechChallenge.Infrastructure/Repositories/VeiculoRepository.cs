using Fiap.TechChallenge.Domain.Entities;
using Fiap.TechChallenge.Domain.Interfaces.Repository;
using Fiap.TechChallenge.Domain.ValueObjects;
using Fiap.TechChallenge.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Fiap.TechChallenge.Infrastructure.Repositories
{
    public class VeiculoRepository(ApplicationDbContext context) : IVeiculoRepository
    {
        private readonly ApplicationDbContext _context = context;

        public async Task<Veiculo?> ObterPorId(Guid id)
        {
            return await _context.Veiculos.FirstOrDefaultAsync(v => v.Id == id);
        }

        public async Task<Veiculo?> ObterPorPlaca(string placa)
        {
            PlacaVeiculoVO placaVO = new(placa);
            return await _context.Veiculos.FirstOrDefaultAsync(v => v.Placa == placaVO);
        }

        public async Task<(IReadOnlyCollection<Veiculo> Items, int TotalCount)> ListarPaginado(int skip, int take)
        {
            var query = _context.Veiculos.OrderBy(v => v.Marca.Valor).ThenBy(v => v.Modelo.Valor);

            int totalCount = await query.CountAsync();

            var items = await query
                .Skip(skip)
                .Take(take)
                .ToListAsync();

            return (items.AsReadOnly(), totalCount);
        }

        public async Task<bool> ExistePlaca(string placa, Guid? ignorarId = null)
        {
            PlacaVeiculoVO placaVO = new(placa);
            return await _context.Veiculos.AnyAsync(v =>
                v.Placa == placaVO &&
                (!ignorarId.HasValue || v.Id != ignorarId.Value)
            );
        }

        public async Task Adicionar(Veiculo veiculo)
        {
            await _context.Veiculos.AddAsync(veiculo);
            await _context.SaveChangesAsync();
        }

        public async Task Atualizar(Veiculo veiculo)
        {
            _context.Veiculos.Update(veiculo);
            await _context.SaveChangesAsync();
        }

        public async Task Deletar(Veiculo veiculo)
        {
            veiculo.MarcarComoApagado();
            _context.Veiculos.Update(veiculo);
            await _context.SaveChangesAsync();
        }
    }
}
