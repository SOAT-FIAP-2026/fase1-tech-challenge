using Fiap.TechChallenge.Domain.Entities;

namespace Fiap.TechChallenge.Domain.Interfaces.Repository
{
    public interface IItemServicoRepository
    {
        Task Adicionar(ItemServico itemServico);
    }
}
