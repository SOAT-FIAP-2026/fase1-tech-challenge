namespace Fiap.TechChallenge.Application.DTOs.Responses
{
    public class VeiculoResponse
    {
        public Guid Id { get; set; }
        public string Placa { get; set; } = string.Empty;
        public string Marca { get; set; } = string.Empty;
        public string Modelo { get; set; } = string.Empty;
        public int Ano { get; set; }

        public VeiculoResponse(Guid id, string placa, string marca, string modelo, int ano)
        {
            Id = id;
            Placa = placa;
            Marca = marca;
            Modelo = modelo;
            Ano = ano;
        }
    }
}
