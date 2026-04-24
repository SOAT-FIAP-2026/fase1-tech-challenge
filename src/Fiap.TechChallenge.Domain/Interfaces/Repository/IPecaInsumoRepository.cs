using Fiap.TechChallenge.Domain.Entities;

namespace Fiap.TechChallenge.Domain.Interfaces.Repository
{
    public interface IPecaInsumoRepository
    {
        Task Adicionar(PecaInsumo pecaInsumo);
    }
}
