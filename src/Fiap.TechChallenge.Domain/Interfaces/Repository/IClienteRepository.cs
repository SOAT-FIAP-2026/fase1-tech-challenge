using Fiap.TechChallenge.Domain.Entities;

namespace Fiap.TechChallenge.Domain.Interfaces.Repository
{
    public interface IClienteRepository
    {
        Task Adicionar(Cliente cliente);
    }
}
