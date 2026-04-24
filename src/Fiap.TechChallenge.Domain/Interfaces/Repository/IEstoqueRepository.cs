using Fiap.TechChallenge.Domain.Entities;

namespace Fiap.TechChallenge.Domain.Interfaces.Repository
{
    public interface IEstoqueRepository
    {
        Task Adicionar(Estoque estoque);
    }
}
