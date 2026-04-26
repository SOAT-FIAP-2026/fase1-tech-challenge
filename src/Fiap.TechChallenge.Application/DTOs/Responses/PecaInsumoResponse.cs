namespace Fiap.TechChallenge.Application.DTOs.Responses
{
    public class PecaInsumoResponse
    {
        public Guid Id { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public decimal ValorUnitario { get; set; }

        public PecaInsumoResponse(Guid id, string descricao, decimal valorUnitario)
        {
            Id = id;
            Descricao = descricao;
            ValorUnitario = valorUnitario;
        }
    }
}
