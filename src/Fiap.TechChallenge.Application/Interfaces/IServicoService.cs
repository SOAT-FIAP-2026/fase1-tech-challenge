using Fiap.TechChallenge.Application.DTOs.Requests;
using Fiap.TechChallenge.Application.DTOs.Responses;

namespace Fiap.TechChallenge.Application.Interfaces
{
    public interface IServicoService
    {
        Task<Guid> Criar(ServicoRequest request);
        Task<ServicoResponse> ObterPorId(Guid id);
        Task<IReadOnlyCollection<ServicoResponse>> ObterTodos();
        Task<IReadOnlyCollection<ServicoMetricaTempoResponse>> ObterMetricasTempo();
        Task<ServicoResponse> Atualizar(Guid id, ServicoRequest request);
        Task Deletar(Guid id);
    }
}
