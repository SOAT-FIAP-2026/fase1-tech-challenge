using Fiap.TechChallenge.Domain.Entities;

namespace Fiap.TechChallenge.Domain.Interfaces.Repository
{
    public interface IOrdemServicoRepository
    {
        Task Adicionar(OrdemServico ordemServico);
        Task Atualizar(OrdemServico ordemServico);
        Task<OrdemServico?> ObterPorId(Guid id);
        Task<IReadOnlyCollection<OrdemServico>> ObterTodos();
        Task<IReadOnlyCollection<ItemServico>> ObterItensServicoFinalizados();
    }
}
