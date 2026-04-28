using Fiap.TechChallenge.Domain.Entities;
using Fiap.TechChallenge.Domain.Interfaces.Repository;
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
    }
}
