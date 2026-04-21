using Fiap.TechChallenge.Application.DTOs.Requests;
using Fiap.TechChallenge.Domain.Entities;

namespace Fiap.TechChallenge.Application.Interfaces
{
    public interface IAutenticacaoService
    {
        Task<Guid> Cadastrar(CadastrarRequest request);
        Task<LoginResponse> Login(LoginRequest request);
    }
}
