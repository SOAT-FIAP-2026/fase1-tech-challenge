using Fiap.TechChallenge.Contracts.CoreService;

namespace Fiap.TechChallenge.Application.DTOs.Responses
{
    public class PecaInsumoResponse
    {
        public Guid Id { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public decimal ValorUnitario { get; set; }

        public PecaInsumoResponse() { }

        public PecaInsumoResponse(Guid id, string descricao, decimal valorUnitario)
        {
            Id = id;
            Descricao = descricao;
            ValorUnitario = valorUnitario;
        }

        public PecaInsumoResponse(PecaInsumoCatalogoDto dto)
            : this(dto.Id, dto.Descricao, dto.ValorUnitario) { }

        public PecaInsumoCatalogoDto ToContract() => new(Id, Descricao, ValorUnitario);

        public static implicit operator PecaInsumoCatalogoDto(PecaInsumoResponse response) => response.ToContract();
        public static implicit operator PecaInsumoResponse(PecaInsumoCatalogoDto dto) => new(dto);
    }
}
