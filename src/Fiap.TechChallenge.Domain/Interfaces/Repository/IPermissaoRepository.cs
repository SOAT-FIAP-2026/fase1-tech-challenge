using Fiap.TechChallenge.Domain.Entities;

namespace Fiap.TechChallenge.Domain.Interfaces.Repository
{
    public interface IPermissaoRepository
    {
        Task Adicionar(Permissao permissao);
    }
}
