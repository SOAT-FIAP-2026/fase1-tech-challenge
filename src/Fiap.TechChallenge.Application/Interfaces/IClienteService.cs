using Fiap.TechChallenge.Application.DTOs.Requests;
using Fiap.TechChallenge.Application.DTOs.Responses;

namespace Fiap.TechChallenge.Application.Interfaces
{
    public interface IClienteService
    {
        Task<Guid> Criar(ClienteRequest request);
        Task<ClienteResponse> ObterPorId(Guid id);
        Task<IReadOnlyCollection<ClienteResponse>> ObterTodos();
        Task<ClienteResponse> Atualizar(Guid id, ClienteRequest request);
        Task Deletar(Guid id);
    }
}
