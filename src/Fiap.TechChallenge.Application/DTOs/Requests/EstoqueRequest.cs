using Fiap.TechChallenge.Contracts.CoreService;

namespace Fiap.TechChallenge.Application.DTOs.Requests
{
    public class EstoqueRequest
    {
        public Guid IdPecaInsumo { get; set; }
        public int Quantidade { get; set; }

        public CriarEstoqueRequest ToContract() => new(IdPecaInsumo, Quantidade);

        public static implicit operator CriarEstoqueRequest(EstoqueRequest request) =>
            new(request.IdPecaInsumo, request.Quantidade);

        public static implicit operator EstoqueRequest(CriarEstoqueRequest contract) =>
            new() { IdPecaInsumo = contract.IdPecaInsumo, Quantidade = contract.Quantidade };
    }
}
