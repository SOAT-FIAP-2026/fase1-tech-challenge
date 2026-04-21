namespace Fiap.TechChallenge.Application.DTOs.Requests
{
    public class LoginResponse
    {
        public string Token { get; set; }
        public string NomeUsuario { get; set; }

        public LoginResponse(string token, string nomeUsuario)
        {
            Token = token;
            NomeUsuario = nomeUsuario;
        }
    }
}