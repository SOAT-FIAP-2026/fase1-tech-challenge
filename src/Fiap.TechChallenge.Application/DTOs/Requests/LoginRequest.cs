using Fiap.TechChallenge.Contracts.AuthService;

namespace Fiap.TechChallenge.Application.DTOs.Requests
{
    public class LoginRequest
    {
        public string Login { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;

        public StaffLoginRequest ToContract() => new(Login, Senha);

        public static implicit operator StaffLoginRequest(LoginRequest request) =>
            new(request.Login, request.Senha);

        public static implicit operator LoginRequest(StaffLoginRequest contract) =>
            new() { Login = contract.Login, Senha = contract.Senha };
    }
}
