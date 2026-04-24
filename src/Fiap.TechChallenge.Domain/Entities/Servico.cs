namespace Fiap.TechChallenge.Domain.Entities
{
    public class Servico
    {
        public Guid Id { get; private set; }
        public string Descricao { get; private set; } = string.Empty;
        public decimal ValorUnitario { get; private set; }

        protected Servico() { }

        public Servico(string descricao, decimal valorUnitario)
        {
            Id = Guid.NewGuid();
            Descricao = descricao;
            ValorUnitario = valorUnitario;
        }
    }
}
