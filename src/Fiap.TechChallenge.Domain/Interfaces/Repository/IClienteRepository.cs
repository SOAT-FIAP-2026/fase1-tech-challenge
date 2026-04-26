using Fiap.TechChallenge.Domain.Entities;

namespace Fiap.TechChallenge.Domain.Interfaces.Repository
{
    public interface IClienteRepository
    {
        Task<Cliente?> ObterPorId(Guid id);
        Task<IReadOnlyCollection<Cliente>> ObterTodos();
        Task<bool> ExisteCpfCnpj(string cpfCnpj, Guid? ignorarId = null);
        Task Adicionar(Cliente cliente);
        Task Atualizar(Cliente cliente);
        Task Deletar(Cliente cliente);
    }
}
