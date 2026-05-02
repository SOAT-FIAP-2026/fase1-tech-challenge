namespace Fiap.TechChallenge.Application.DTOs.Responses
{
    public class OrdemServicoItemServicoResponse( Guid idServico, DateTime? dataHoraInicio, DateTime? dataHoraFim, string descricao, decimal valor)
    {
        
        public Guid IdServico { get; set; } = idServico;
        public string Descricao { get; set; } = descricao;
        public decimal Valor { get; set; } = valor;
        public DateTime? DataHoraInicio { get; set; } = dataHoraInicio;
        public DateTime? DataHoraFim { get; set; } = dataHoraFim;
    }
}