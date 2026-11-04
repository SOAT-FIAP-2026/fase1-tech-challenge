using Fiap.TechChallenge.Contracts.CoreService;

namespace Fiap.TechChallenge.Application.DTOs.Responses
{
    public class VeiculoResponse
    {
        public Guid Id { get; set; }
        public string Placa { get; set; } = string.Empty;
        public string Marca { get; set; } = string.Empty;
        public string Modelo { get; set; } = string.Empty;
        public int Ano { get; set; }

        public VeiculoResponse() { }

        public VeiculoResponse(Guid id, string placa, string marca, string modelo, int ano)
        {
            Id = id;
            Placa = placa;
            Marca = marca;
            Modelo = modelo;
            Ano = ano;
        }

        public VeiculoResponse(VeiculoCatalogoDto dto)
            : this(dto.Id, dto.Placa, dto.Marca, dto.Modelo, dto.Ano) { }

        public VeiculoCatalogoDto ToContract() => new(Id, Placa, Marca, Modelo, Ano);

        public static implicit operator VeiculoCatalogoDto(VeiculoResponse response) => response.ToContract();
        public static implicit operator VeiculoResponse(VeiculoCatalogoDto dto) => new(dto);
    }
}
