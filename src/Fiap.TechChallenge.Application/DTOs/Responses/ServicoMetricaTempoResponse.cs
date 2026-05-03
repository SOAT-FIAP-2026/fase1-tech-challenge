namespace Fiap.TechChallenge.Application.DTOs.Responses
{
    public class ServicoMetricaTempoResponse(
        Guid idServico,
        string nome,
        int tempoEstimadoMinutos,
        int? tempoMedioExecucaoMinutos,
        int quantidadeExecucoes)
    {
        public Guid IdServico { get; set; } = idServico;
        public string Nome { get; set; } = nome;
        public int TempoEstimadoMinutos { get; set; } = tempoEstimadoMinutos;
        public int? TempoMedioExecucaoMinutos { get; set; } = tempoMedioExecucaoMinutos;
        public int QuantidadeExecucoes { get; set; } = quantidadeExecucoes;
    }
}
