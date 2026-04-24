using Fiap.TechChallenge.Domain.Entities;

namespace Fiap.TechChallenge.Domain.Interfaces.Repository
{
    public interface IServicoRepository
    {
        Task Adicionar(Servico servico);
    }
}
