using Fiap.TechChallenge.Domain.Entities;

namespace Fiap.TechChallenge.Domain.Interfaces.Repository
{
    public interface IVeiculoRepository
    {
        Task<Veiculo?> ObterPorId(Guid id);
        Task<Veiculo?> ObterPorPlaca(string placa);
        Task<(IReadOnlyCollection<Veiculo> Items, int TotalCount)> ListarPaginado(int skip, int take);
        Task<bool> ExistePlaca(string placa, Guid? ignorarId = null);
        Task Adicionar(Veiculo veiculo);
        Task Atualizar(Veiculo veiculo);
        Task Deletar(Veiculo veiculo);
    }
}
