namespace Fiap.TechChallenge.Application.DTOs.Responses
{
    public class OrdemServicoItemPecaInsumoResponse(Guid idPecaInsumo, string descricao, decimal valor)
    {
        public Guid IdPecaInsumo { get; set; } = idPecaInsumo;
        public string Descricao { get; set; } = descricao;
        public decimal Valor { get; set; } = valor;
    }
}