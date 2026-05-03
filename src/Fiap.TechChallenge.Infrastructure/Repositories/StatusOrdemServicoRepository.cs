using Fiap.TechChallenge.Domain.Entities;
using Fiap.TechChallenge.Domain.Interfaces.Repository;
using Fiap.TechChallenge.Domain.ValueObjects;
using Fiap.TechChallenge.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Fiap.TechChallenge.Infrastructure.Repositories
{
    public class StatusOrdemServicoRepository(ApplicationDbContext context) : IStatusOrdemServicoRepository
    {
        private readonly ApplicationDbContext _context = context;

        public async Task<StatusOrdemServico?> ObterPorCodigo(CodigoVO codigo)
        {
            return await _context.StatusOrdensServico.FirstOrDefaultAsync(s => s.Codigo == codigo);
        }
    }
}
