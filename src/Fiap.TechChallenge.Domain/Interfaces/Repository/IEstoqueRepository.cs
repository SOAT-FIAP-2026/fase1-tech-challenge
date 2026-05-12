using Fiap.TechChallenge.Domain.Entities;

namespace Fiap.TechChallenge.Domain.Interfaces.Repository
{
    public interface IEstoqueRepository
    {
        Task<int?> VerificarQuantidadePorIdPecaInsumo(Guid idPecaInsumo);
        Task<int?> VerificarQuantidadePorDescricaoPeca(string descricao);
        Task<Estoque?> ObterPorId(Guid idEstoque);
        Task<Estoque?> ObterPorIdPecaInsumo(Guid idPecaInsumo);
        Task Adicionar(Estoque estoque);
        Task Atualizar(Estoque estoque);
        Task Deletar(Guid idEstoque);
    }
}
