namespace Fiap.TechChallenge.Application.DTOs.Responses
{
    public class OrdemServicoItemPecaInsumoResponse(Guid id, Guid idPecaInsumo/* string pecaInsumoDescricao*/)
    {
        public Guid Id { get; set; } = id;
        public Guid IdPecaInsumo { get; set; } = idPecaInsumo;
        // public string PecaInsumoDescricao { get; set; } = pecaInsumoDescricao;
    }
}