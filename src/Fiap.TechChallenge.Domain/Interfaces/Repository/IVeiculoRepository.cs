using Fiap.TechChallenge.Domain.Entities;

namespace Fiap.TechChallenge.Domain.Interfaces.Repository
{
    public interface IVeiculoRepository
    {
        Task Adicionar(Veiculo veiculo);
    }
}
