namespace Fiap.TechChallenge.Application.DTOs.Responses
{
    public class ClienteResponse
    {
        public Guid Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string CpfCnpj { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Celular { get; set; } = string.Empty;

        public ClienteResponse(Guid id, string nome, string cpfCnpj, string email, string celular)
        {
            Id = id;
            Nome = nome;
            CpfCnpj = cpfCnpj;
            Email = email;
            Celular = celular;
        }
    }
}
