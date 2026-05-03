namespace Fiap.TechChallenge.Application.DTOs.Responses
{
    public class OrdemServicoProgressoServicoResponse(
        Guid idServico,
        string nome,
        string status,
        int tempoEstimadoMinutos,
        int? tempoExecutadoMinutos,
        DateTime? dataHoraInicio,
        DateTime? dataHoraFim)
    {
        public Guid IdServico { get; set; } = idServico;
        public string Nome { get; set; } = nome;
        public string Status { get; set; } = status;
        public int TempoEstimadoMinutos { get; set; } = tempoEstimadoMinutos;
        public int? TempoExecutadoMinutos { get; set; } = tempoExecutadoMinutos;
        public DateTime? DataHoraInicio { get; set; } = dataHoraInicio;
        public DateTime? DataHoraFim { get; set; } = dataHoraFim;
    }
}
