namespace Fiap.TechChallenge.Application.DTOs.Requests
{
    public class ClienteRequest
    {
        public string Nome { get; set; } = string.Empty;
        public string CpfCnpj { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Celular { get; set; } = string.Empty;
    }
}
