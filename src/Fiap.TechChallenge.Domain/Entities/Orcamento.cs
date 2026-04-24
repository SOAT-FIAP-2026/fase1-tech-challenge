namespace Fiap.TechChallenge.Domain.Entities
{
    public class Orcamento
    {
        public Guid Id { get; private set; }
        public Guid IdOrdemServico { get; private set; }
        public decimal ValorTotal { get; private set; }

        protected Orcamento() { }

        public Orcamento(Guid idOrdemServico, decimal valorTotal)
        {
            Id = Guid.NewGuid();
            IdOrdemServico = idOrdemServico;
            ValorTotal = valorTotal;
        }
    }
}
