using Fiap.TechChallenge.Application.DTOs.Requests;
using Fiap.TechChallenge.Domain.Entities;

namespace Fiap.TechChallenge.Application.Interfaces
{
    public interface IServicoService
    {
        Task<Guid> Criar(ServicoRequest request);
    }
}
