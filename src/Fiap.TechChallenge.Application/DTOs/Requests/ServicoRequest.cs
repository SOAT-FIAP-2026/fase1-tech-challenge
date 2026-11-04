using Fiap.TechChallenge.Contracts.CoreService;

namespace Fiap.TechChallenge.Application.DTOs.Requests
{
    public class ServicoRequest
    {
        public string Nome { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public decimal ValorUnitario { get; set; }
        public int TempoEstimadoMinutos { get; set; } = 60;

        public CriarServicoRequest ToContract() => new(Nome, Descricao, ValorUnitario, TempoEstimadoMinutos);

        public static implicit operator CriarServicoRequest(ServicoRequest request) =>
            new(request.Nome, request.Descricao, request.ValorUnitario, request.TempoEstimadoMinutos);

        public static implicit operator ServicoRequest(CriarServicoRequest contract) =>
            new() { Nome = contract.Nome, Descricao = contract.Descricao, ValorUnitario = contract.ValorUnitario, TempoEstimadoMinutos = contract.TempoEstimadoMinutos };
    }
}
