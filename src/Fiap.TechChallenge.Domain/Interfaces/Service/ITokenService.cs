using Fiap.TechChallenge.Domain.Entities;
    
namespace Fiap.TechChallenge.Domain.Interfaces.Service
{
    public interface ITokenService
    {
        string GerarToken(Usuario usuario);
    }
}
