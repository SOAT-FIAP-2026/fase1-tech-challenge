namespace Fiap.TechChallenge.Application.DTOs.Responses
{
    public class ServicoResponse
    {
        public Guid Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public decimal ValorUnitario { get; set; }
        public int TempoEstimadoMinutos { get; set; }

        public ServicoResponse(Guid id, string nome, string descricao, decimal valorUnitario, int tempoEstimadoMinutos = 60)
        {
            Id = id;
            Nome = nome;
            Descricao = descricao;
            ValorUnitario = valorUnitario;
            TempoEstimadoMinutos = tempoEstimadoMinutos;
        }
    }
}
