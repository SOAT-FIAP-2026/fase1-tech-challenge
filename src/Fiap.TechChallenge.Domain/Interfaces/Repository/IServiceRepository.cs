using Fiap.TechChallenge.Domain.Entities;

namespace Fiap.TechChallenge.Domain.Interfaces.Repository
{
    public interface IServicoRepository
    {
        Task<Servico?> ObterPorId(Guid id);
        Task<IReadOnlyCollection<Servico>> ObterTodos();
        Task<bool> ExisteNome(string nome, Guid? ignorarId = null);
        Task Adicionar(Servico servico);
        Task Atualizar(Servico servico);
        Task Deletar(Servico servico);
    }
}
