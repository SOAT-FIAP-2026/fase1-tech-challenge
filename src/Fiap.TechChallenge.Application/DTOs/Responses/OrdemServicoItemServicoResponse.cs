namespace Fiap.TechChallenge.Application.DTOs.Responses
{
    public class OrdemServicoItemServicoResponse
    {
        public Guid Id { get; set; }
        public Guid IdServico { get; set; }
        public DateTime? DataHoraInicio { get; set; }
        public DateTime? DataHoraFim { get; set; }

        public OrdemServicoItemServicoResponse(Guid id, Guid idServico, DateTime? dataHoraInicio, DateTime? dataHoraFim)
        {
            Id = id;
            IdServico = idServico;
            DataHoraInicio = dataHoraInicio;
            DataHoraFim = dataHoraFim;
        }
    }
}