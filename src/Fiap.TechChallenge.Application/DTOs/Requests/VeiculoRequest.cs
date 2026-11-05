using Fiap.TechChallenge.Contracts.CoreService;

namespace Fiap.TechChallenge.Application.DTOs.Requests
{
    public class VeiculoRequest
    {
        public string Placa { get; set; } = string.Empty;
        public string Marca { get; set; } = string.Empty;
        public string Modelo { get; set; } = string.Empty;
        public int Ano { get; set; }

        public CriarVeiculoRequest ToContract() => new(Placa, Marca, Modelo, Ano);

        public static implicit operator CriarVeiculoRequest(VeiculoRequest request) =>
            new(request.Placa, request.Marca, request.Modelo, request.Ano);

        public static implicit operator VeiculoRequest(CriarVeiculoRequest contract) =>
            new() { Placa = contract.Placa, Marca = contract.Marca, Modelo = contract.Modelo, Ano = contract.Ano };
    }
}
