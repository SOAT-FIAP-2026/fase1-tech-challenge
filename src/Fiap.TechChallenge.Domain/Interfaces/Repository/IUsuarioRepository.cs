using Fiap.TechChallenge.Domain.Entities;
using Fiap.TechChallenge.Domain.ValueObjects;

namespace Fiap.TechChallenge.Domain.Interfaces.Repository
{
    public interface IUsuarioRepository
    {
        Task<Usuario?> ObterPorLogin(EmailVO email);
        Task<bool> ExisteEmail(EmailVO email);
        Task Adicionar(Usuario usuario);
    }
}
