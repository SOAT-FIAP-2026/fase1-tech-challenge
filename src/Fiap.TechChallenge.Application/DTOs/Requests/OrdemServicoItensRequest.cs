namespace Fiap.TechChallenge.Application.DTOs.Requests
{
    public class OrdemServicoItensRequest
    {
        public List<Guid> ServicosIds { get; set; } = [];
        public List<Guid> PecasInsumoIds { get; set; } = [];
    }
}
