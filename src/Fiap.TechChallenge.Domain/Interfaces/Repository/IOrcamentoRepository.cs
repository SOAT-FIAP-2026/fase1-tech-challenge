using Fiap.TechChallenge.Domain.Entities;

namespace Fiap.TechChallenge.Domain.Interfaces.Repository
{
    public interface IOrcamentoRepository
    {
        Task Adicionar(Orcamento orcamento);
    }
}
