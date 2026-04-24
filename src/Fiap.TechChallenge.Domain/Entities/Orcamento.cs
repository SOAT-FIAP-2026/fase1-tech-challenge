using Fiap.TechChallenge.Domain.ValueObjects;

namespace Fiap.TechChallenge.Domain.Entities
{
    public class Orcamento : EntidadeAuditavel
    {
        public Guid IdOrdemServico { get; private set; }
        public ValorMonetarioVO ValorTotal { get; private set; } = null!;
        public OrdemServico OrdemServico { get; private set; } = null!;

        protected Orcamento() { }

        public Orcamento(Guid idOrdemServico, decimal valorTotal) : base()
        {
            IdOrdemServico = idOrdemServico;
            ValorTotal = new ValorMonetarioVO(valorTotal);
        }

        public void AlterarValorTotal(decimal valor)
        {
            ValorTotal = new ValorMonetarioVO(valor);
            AtualizarTimestamp();
        }
    }
}
