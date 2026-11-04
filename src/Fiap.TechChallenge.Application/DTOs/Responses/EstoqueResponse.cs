using Fiap.TechChallenge.Contracts.CoreService;

namespace Fiap.TechChallenge.Application.DTOs.Responses
{
    public class EstoqueResponse
    {
        public Guid Id { get; set; }
        public Guid IdPecaInsumo { get; set; }
        public int Quantidade { get; set; }

        public EstoqueResponse() { }

        public EstoqueResponse(Guid id, Guid idPecaInsumo, int quantidade)
        {
            Id = id;
            IdPecaInsumo = idPecaInsumo;
            Quantidade = quantidade;
        }

        public EstoqueResponse(EstoqueDetalhadoDto dto)
            : this(dto.Id, dto.IdPecaInsumo, dto.Quantidade) { }

        public EstoqueDetalhadoDto ToContract() => new(Id, IdPecaInsumo, Quantidade);

        public static implicit operator EstoqueDetalhadoDto(EstoqueResponse response) => response.ToContract();
        public static implicit operator EstoqueResponse(EstoqueDetalhadoDto dto) => new(dto);
    }
}
