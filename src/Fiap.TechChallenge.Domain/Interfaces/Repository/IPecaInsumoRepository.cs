using Fiap.TechChallenge.Domain.Entities;

namespace Fiap.TechChallenge.Domain.Interfaces.Repository {
    public interface IPecaInsumoRepository
    {
        Task<PecaInsumo?> ObterPorId(Guid id);
        Task<PecaInsumo?> ObterPorDescricao(string descricao);
        Task<IReadOnlyCollection<PecaInsumo>> ListarTodos();
        Task<IReadOnlyCollection<PecaInsumo>> ObterPorIds(IReadOnlyCollection<Guid> ids);
        Task Adicionar(PecaInsumo pecaInsumo);
        Task Atualizar(PecaInsumo pecaInsumo);
        Task Deletar(PecaInsumo pecaInsumo);
    }
}