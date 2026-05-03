namespace Fiap.TechChallenge.Application.DTOs.Requests
{
    public class VeiculoRequest
    {
        public string Placa { get; set; } = string.Empty;
        public string Marca { get; set; } = string.Empty;
        public string Modelo { get; set; } = string.Empty;
        public int Ano { get; set; }
    }
}
