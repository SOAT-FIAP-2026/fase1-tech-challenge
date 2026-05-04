using Fiap.TechChallenge.Application.DTOs.Responses;
using Fiap.TechChallenge.Application.DTOs.Requests;

namespace Fiap.TechChallenge.Application.Interfaces
{
    public interface IOrdemServicoService
    {
        Task<Guid> Criar(OrdemServicoRequest request);
        Task<OrdemServicoResponse> IniciarDiagnostico(Guid id);
        Task<OrdemServicoResponse> FinalizarDiagnostico(Guid id);
        Task<OrdemServicoResponse> IncluirServico(Guid id, OrdemServicoServicosRequest request);
        Task<OrdemServicoResponse> IncluirPecaInsumo(Guid id, OrdemServicoPecaInsumoRequest request);
        Task RemoverItemServico(Guid id, Guid idServico);
        Task RemoverItemPecaInsumo(Guid id, Guid idPecaInsumo);
        Task IniciarServico(Guid id, Guid idServico);
        Task FinalizarServico(Guid id, Guid idServico);
        Task<OrdemServicoResponse> AprovarOrdemServico(Guid id);
        Task<OrdemServicoResponse> ObterPorId(Guid id);
        Task<OrdemServicoProgressoResponse> ObterProgresso(Guid id);
        Task<IReadOnlyCollection<OrdemServicoResponse>> ObterTodos();
    }
}
