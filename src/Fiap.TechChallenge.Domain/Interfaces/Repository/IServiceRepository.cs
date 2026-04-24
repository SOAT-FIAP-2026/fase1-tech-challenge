using Fiap.TechChallenge.Domain.Entities;

namespace Fiap.TechChallenge.Domain.Interfaces.Repository
{
    public interface IServicoRepository
    {
        Task<Servico?> ObterPorId(Guid id);
        Task Adicionar(Servico servico);
    }
}
