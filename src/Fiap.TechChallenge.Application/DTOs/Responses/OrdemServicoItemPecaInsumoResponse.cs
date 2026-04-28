namespace Fiap.TechChallenge.Application.DTOs.Responses
{
    public class OrdemServicoItemPecaInsumoResponse
    {
        public Guid Id { get; set; }
        public Guid IdPecaInsumo { get; set; }

        public OrdemServicoItemPecaInsumoResponse(Guid id, Guid idPecaInsumo)
        {
            Id = id;
            IdPecaInsumo = idPecaInsumo;
        }
    }
}