using Fiap.TechChallenge.Contracts.CoreService;

namespace Fiap.TechChallenge.Application.DTOs.Responses
{
    public class ClienteResponse
    {
        public Guid Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string CpfCnpj { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Celular { get; set; } = string.Empty;

        public ClienteResponse() { }

        public ClienteResponse(Guid id, string nome, string cpfCnpj, string email, string celular)
        {
            Id = id;
            Nome = nome;
            CpfCnpj = cpfCnpj;
            Email = email;
            Celular = celular;
        }

        public ClienteResponse(ClienteDetalhadoDto dto)
            : this(dto.Id, dto.Nome, dto.CpfCnpj, dto.Email, dto.Celular) { }

        public ClienteResponse(ClienteCatalogoDto dto)
            : this(dto.Id, dto.Nome, string.Empty, dto.Email, dto.Celular ?? string.Empty) { }

        public ClienteDetalhadoDto ToContract() => new(Id, Nome, CpfCnpj, Email, Celular);
        public ClienteCatalogoDto ToCatalogoContract() => new(Id, Nome, Email, Celular);

        public static implicit operator ClienteDetalhadoDto(ClienteResponse response) => response.ToContract();
        public static implicit operator ClienteResponse(ClienteDetalhadoDto dto) => new(dto);
        public static implicit operator ClienteCatalogoDto(ClienteResponse response) => response.ToCatalogoContract();
        public static implicit operator ClienteResponse(ClienteCatalogoDto dto) => new(dto);
    }
}
