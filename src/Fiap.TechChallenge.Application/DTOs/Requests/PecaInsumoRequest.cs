using Fiap.TechChallenge.Contracts.CoreService;

namespace Fiap.TechChallenge.Application.DTOs.Requests
{
    public class PecaInsumoRequest
    {
        public string Descricao { get; set; } = string.Empty;
        public decimal ValorUnitario { get; set; }

        public CriarPecaInsumoRequest ToContract() => new(Descricao, ValorUnitario);

        public static implicit operator CriarPecaInsumoRequest(PecaInsumoRequest request) =>
            new(request.Descricao, request.ValorUnitario);

        public static implicit operator PecaInsumoRequest(CriarPecaInsumoRequest contract) =>
            new() { Descricao = contract.Descricao, ValorUnitario = contract.ValorUnitario };
    }
}
