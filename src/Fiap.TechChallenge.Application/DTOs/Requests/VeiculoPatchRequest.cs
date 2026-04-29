namespace Fiap.TechChallenge.Application.DTOs.Requests
{
    public class VeiculoPatchRequest
    {
        public string? Placa { get; set; }
        public string? Marca { get; set; }
        public string? Modelo { get; set; }
        public int? Ano { get; set; }
    }
}
