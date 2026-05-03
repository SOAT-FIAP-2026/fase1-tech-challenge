namespace Fiap.TechChallenge.Application.DTOs.Responses
{
    public class OrdemServicoProgressoResponse(
        Guid idOrdemServico,
        string status,
        int percentualConcluido,
        DateTime dataAbertura,
        DateTime? previsaoConclusao,
        IReadOnlyCollection<OrdemServicoProgressoServicoResponse> servicos)
    {
        public Guid IdOrdemServico { get; set; } = idOrdemServico;
        public string Status { get; set; } = status;
        public int PercentualConcluido { get; set; } = percentualConcluido;
        public DateTime DataAbertura { get; set; } = dataAbertura;
        public DateTime? PrevisaoConclusao { get; set; } = previsaoConclusao;
        public IReadOnlyCollection<OrdemServicoProgressoServicoResponse> Servicos { get; set; } = servicos;
    }
}
