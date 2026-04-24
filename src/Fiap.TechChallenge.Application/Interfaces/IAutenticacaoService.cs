using Fiap.TechChallenge.Application.DTOs.Requests;

namespace Fiap.TechChallenge.Application.Interfaces
{
    public interface IAutenticacaoService
    {
        Task<Guid> Cadastrar(CadastrarRequest request);
        Task<LoginResponse> Login(LoginRequest request);
    }
}
