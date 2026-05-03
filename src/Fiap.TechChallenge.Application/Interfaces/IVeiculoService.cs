using Fiap.TechChallenge.Application.DTOs.Common;
using Fiap.TechChallenge.Application.DTOs.Requests;
using Fiap.TechChallenge.Application.DTOs.Responses;

namespace Fiap.TechChallenge.Application.Interfaces
{
    public interface IVeiculoService
    {
        Task<Guid> Criar(VeiculoRequest request);
        Task<VeiculoResponse> ObterPorId(Guid id);
        Task<PagedResult<VeiculoResponse>> ListarPaginado(PagedRequest request);
        Task<VeiculoResponse> Atualizar(Guid id, VeiculoPatchRequest request);
        Task Deletar(Guid id);
    }
}
