using Fiap.TechChallenge.Domain.Entities;

namespace Fiap.TechChallenge.Domain.Interfaces.Repository
{
    public interface IItemPecaInsumoRepository
    {
        Task Adicionar(ItemPecaInsumo itemPecaInsumo);
    }
}
