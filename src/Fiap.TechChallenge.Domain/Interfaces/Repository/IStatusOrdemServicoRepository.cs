using Fiap.TechChallenge.Domain.Entities;

namespace Fiap.TechChallenge.Domain.Interfaces.Repository
{
    public interface IStatusOrdemServicoRepository
    {
        Task Adicionar(StatusOrdemServico statusOrdemServico);
    }
}
