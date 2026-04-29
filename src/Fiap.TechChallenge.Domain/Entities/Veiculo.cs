using Fiap.TechChallenge.Domain.ValueObjects;

namespace Fiap.TechChallenge.Domain.Entities
{
    public class Veiculo : EntidadeAuditavel
    {
        public PlacaVeiculoVO Placa { get; private set; } = null!;
        public NomeVO Marca { get; private set; } = null!;
        public NomeVO Modelo { get; private set; } = null!;
        public AnoVeiculoVO Ano { get; private set; } = null!;

        protected Veiculo() { }

        public Veiculo(string placa, string marca, string modelo, int ano) : base()
        {
            Placa = new PlacaVeiculoVO(placa);
            Marca = new NomeVO(marca);
            Modelo = new NomeVO(modelo);
            Ano = new AnoVeiculoVO(ano);
        }

        public void AlterarPlaca(string placa)
        {
            Placa = new PlacaVeiculoVO(placa);
            AtualizarTimestamp();
        }

        public void AlterarMarca(string marca)
        {
            Marca = new NomeVO(marca);
            AtualizarTimestamp();
        }

        public void AlterarModelo(string modelo)
        {
            Modelo = new NomeVO(modelo);
            AtualizarTimestamp();
        }

        public void AlterarAno(int ano)
        {
            Ano = new AnoVeiculoVO(ano);
            AtualizarTimestamp();
        }
    }
}
