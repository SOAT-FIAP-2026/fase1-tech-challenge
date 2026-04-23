using Fiap.TechChallenge.Domain.Entities;

namespace Fiap.TechChallenge.Domain.Interfaces.Repository
{
    public interface IUsuarioRepository
    {
        Task<Usuario?> ObterPorLogin(string login);
        Task<bool> ExisteEmail(string email);
        Task Adicionar(Usuario usuario);
    }
}
