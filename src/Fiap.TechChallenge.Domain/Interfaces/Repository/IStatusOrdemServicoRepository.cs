using Fiap.TechChallenge.Domain.Entities;
using Fiap.TechChallenge.Domain.ValueObjects;

namespace Fiap.TechChallenge.Domain.Interfaces.Repository
{
    public interface IStatusOrdemServicoRepository
    {
        Task<StatusOrdemServico?> ObterPorCodigo(CodigoVO codigo);
    }
}
