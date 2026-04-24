using Fiap.TechChallenge.Domain.ValueObjects;

namespace Fiap.TechChallenge.Domain.Entities
{
    public class Veiculo : EntidadeAuditavel
    {
        public PlacaVeiculoVO Placa { get; private set; } = null!;
        public string Marca { get; private set; } = null!;
        public string Modelo { get; private set; } = null!;
        public int Ano { get; private set; }

        protected Veiculo() { }

        public Veiculo(string placa, string marca, string modelo, int ano) : base()
        {
            if (string.IsNullOrWhiteSpace(marca))
                throw new ArgumentException("A marca não pode ser vazia.");
            if (string.IsNullOrWhiteSpace(modelo))
                throw new ArgumentException("O modelo não pode ser vazio.");
            if (ano < 1900 || ano > DateTime.UtcNow.Year + 1)
                throw new ArgumentException("Ano do veículo inválido.");

            Placa = new PlacaVeiculoVO(placa);
            Marca = marca.Trim();
            Modelo = modelo.Trim();
            Ano = ano;
        }

        public void AlterarPlaca(string placa)
        {
            Placa = new PlacaVeiculoVO(placa);
            AtualizarTimestamp();
        }
    }
}
