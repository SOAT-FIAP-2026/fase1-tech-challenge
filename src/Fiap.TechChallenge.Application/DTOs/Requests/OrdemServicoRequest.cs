namespace Fiap.TechChallenge.Application.DTOs.Requests
{
    public class OrdemServicoRequest
    {
        public Guid ClienteId { get; set; }
        public Guid VeiculoId { get; set; }
        public string? Observacao { get; set; }
    }
}
