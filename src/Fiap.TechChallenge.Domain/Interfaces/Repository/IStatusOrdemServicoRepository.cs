using Fiap.TechChallenge.Domain.Entities;

namespace Fiap.TechChallenge.Domain.Interfaces.Repository
{
    public interface IStatusOrdemServicoRepository
    {
        Task<StatusOrdemServico?> ObterPorDescricao(string descricao);
    }
}
