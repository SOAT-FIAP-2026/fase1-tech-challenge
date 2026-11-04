using Fiap.TechChallenge.Contracts.AuthService;

namespace Fiap.TechChallenge.Application.DTOs.Requests
{
    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public string NomeUsuario { get; set; } = string.Empty;

        public LoginResponse() { }

        public LoginResponse(string token, string nomeUsuario)
        {
            Token = token;
            NomeUsuario = nomeUsuario;
        }

        public LoginResponse(StaffLoginResponse dto)
            : this(dto.Token, dto.NomeUsuario) { }

        public StaffLoginResponse ToContract() => new(Token, NomeUsuario);

        public static implicit operator StaffLoginResponse(LoginResponse response) => response.ToContract();
        public static implicit operator LoginResponse(StaffLoginResponse dto) => new(dto);
    }
}