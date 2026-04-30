using Fiap.TechChallenge.Application.DTOs.Responses;
using Fiap.TechChallenge.Application.DTOs.Requests;

namespace Fiap.TechChallenge.Application.Interfaces
{
    public interface IOrdemServicoService
    {
        Task<Guid> Criar(OrdemServicoRequest request);
        Task<OrdemServicoResponse> IncluirItens(Guid id, OrdemServicoItensRequest request);
        Task<OrdemServicoResponse> ObterPorId(Guid id);
        Task<IReadOnlyCollection<OrdemServicoResponse>> ObterTodos();
    }
}
