using Fiap.TechChallenge.Domain.Entities;

namespace Fiap.TechChallenge.Domain.Interfaces.Repository
{
    public interface IOrdemServicoRepository
    {
        Task Adicionar(OrdemServico ordemServico);
    }
}
