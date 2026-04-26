using Fiap.TechChallenge.Application.DTOs.Requests;
using Fiap.TechChallenge.Application.DTOs.Responses;

namespace Fiap.TechChallenge.Application.Interfaces
{
    public interface IPecaInsumoService
    {
        Task<Guid> Criar(PecaInsumoRequest request);
        Task<PecaInsumoResponse> ObterPorId(Guid id);
        Task<PecaInsumoResponse> ObterPorDescricao(string descricao);
        Task<IReadOnlyCollection<PecaInsumoResponse>> ObterTodos();
        Task<PecaInsumoResponse> Atualizar(Guid id, PecaInsumoRequest request);
        Task Deletar(Guid id);
    }
}
