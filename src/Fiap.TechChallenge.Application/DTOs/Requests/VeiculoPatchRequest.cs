using Fiap.TechChallenge.Contracts.CoreService;

namespace Fiap.TechChallenge.Application.DTOs.Requests
{
    public class VeiculoPatchRequest
    {
        public string? Placa { get; set; }
        public string? Marca { get; set; }
        public string? Modelo { get; set; }
        public int? Ano { get; set; }

        public AtualizarVeiculoPatchRequest ToContract() => new(Placa, Marca, Modelo, Ano);

        public static implicit operator AtualizarVeiculoPatchRequest(VeiculoPatchRequest request) =>
            new(request.Placa, request.Marca, request.Modelo, request.Ano);

        public static implicit operator VeiculoPatchRequest(AtualizarVeiculoPatchRequest contract) =>
            new() { Placa = contract.Placa, Marca = contract.Marca, Modelo = contract.Modelo, Ano = contract.Ano };
    }
}
