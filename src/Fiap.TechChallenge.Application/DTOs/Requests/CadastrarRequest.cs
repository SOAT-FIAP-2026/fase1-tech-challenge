using Fiap.TechChallenge.Contracts.AuthService;

namespace Fiap.TechChallenge.Application.DTOs.Requests
{
    public class CadastrarRequest
    {
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Login { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
        public Guid IdPermissao { get; set; }

        public StaffCadastrarRequest ToContract() => new(Nome, Email, Login, Senha, IdPermissao);

        public static implicit operator StaffCadastrarRequest(CadastrarRequest request) =>
            new(request.Nome, request.Email, request.Login, request.Senha, request.IdPermissao);

        public static implicit operator CadastrarRequest(StaffCadastrarRequest contract) =>
            new()
            {
                Nome = contract.Nome,
                Email = contract.Email,
                Login = contract.Login,
                Senha = contract.Senha,
                IdPermissao = contract.IdPermissao
            };
    }
}
