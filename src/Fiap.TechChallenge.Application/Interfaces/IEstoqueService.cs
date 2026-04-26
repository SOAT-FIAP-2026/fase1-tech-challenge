using Fiap.TechChallenge.Application.DTOs.Requests;
using Fiap.TechChallenge.Application.DTOs.Responses;

namespace Fiap.TechChallenge.Application.Interfaces
{
    public interface IEstoqueService
    {
        Task<Guid> Criar(EstoqueRequest request);
        Task<int?> VerificarQuantidadePorIdPecaInsumo(Guid idPecaInsumo);
        Task<int?> VerificarQuantidadePorDescricaoPeca(string descricao);
        Task AdicionarQuantidade(Guid idPecaInsumo, int quantidade);
        Task RemoverQuantidade(Guid idPecaInsumo, int quantidade);
        Task Deletar(Guid idEstoque);
    }
}
