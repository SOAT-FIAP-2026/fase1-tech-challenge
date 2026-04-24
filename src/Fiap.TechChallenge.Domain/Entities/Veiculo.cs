namespace Fiap.TechChallenge.Domain.Entities
{
    public class Veiculo
    {
        public Guid Id { get; private set; }
        public string Placa { get; private set; } = string.Empty;
        public string Marca { get; private set; } = string.Empty;
        public string Modelo { get; private set; } = string.Empty;
        public int Ano { get; private set; }

        protected Veiculo() { }

        public Veiculo(string placa, string marca, string modelo, int ano)
        {
            Id = Guid.NewGuid();
            Placa = placa;
            Marca = marca;
            Modelo = modelo;
            Ano = ano;
        }
    }
}
