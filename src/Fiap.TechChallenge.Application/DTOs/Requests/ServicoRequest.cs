namespace Fiap.TechChallenge.Application.DTOs.Requests
{
    public class ServicoRequest
    {
        public string Nome { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public decimal ValorUnitario { get; set; }
        public int TempoEstimadoMinutos { get; set; } = 60;
    }
}
