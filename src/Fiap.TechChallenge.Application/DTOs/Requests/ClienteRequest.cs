using Fiap.TechChallenge.Contracts.CoreService;

namespace Fiap.TechChallenge.Application.DTOs.Requests
{
    public class ClienteRequest
    {
        public string Nome { get; set; } = string.Empty;
        public string CpfCnpj { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Celular { get; set; } = string.Empty;

        public CriarClienteRequest ToCriarRequest() => new(Nome, CpfCnpj, Email, Celular);
        public AtualizarClienteRequest ToAtualizarRequest() => new(Nome, CpfCnpj, Email, Celular);

        public static implicit operator CriarClienteRequest(ClienteRequest request) =>
            new(request.Nome, request.CpfCnpj, request.Email, request.Celular);

        public static implicit operator ClienteRequest(CriarClienteRequest contract) =>
            new() { Nome = contract.Nome, CpfCnpj = contract.CpfCnpj, Email = contract.Email, Celular = contract.Celular };

        public static implicit operator AtualizarClienteRequest(ClienteRequest request) =>
            new(request.Nome, request.CpfCnpj, request.Email, request.Celular);

        public static implicit operator ClienteRequest(AtualizarClienteRequest contract) =>
            new() { Nome = contract.Nome, CpfCnpj = contract.CpfCnpj, Email = contract.Email, Celular = contract.Celular };
    }
}
