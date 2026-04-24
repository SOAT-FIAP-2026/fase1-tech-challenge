using Fiap.TechChallenge.Domain.ValueObjects;

namespace Fiap.TechChallenge.Domain.Entities
{
    public class Servico : EntidadeAuditavel
    {
        public DescricaoVO Descricao { get; private set; } = null!;
        public ValorMonetarioVO ValorUnitario { get; private set; } = null!;

        protected Servico() { }

        public Servico(string descricao, decimal valorUnitario) : base()
        {
            Descricao = new DescricaoVO(descricao, 255);
            ValorUnitario = new ValorMonetarioVO(valorUnitario);
        }

        public void AlterarValorUnitario(decimal valor)
        {
            ValorUnitario = new ValorMonetarioVO(valor);
            AtualizarTimestamp();
        }

        public void AlterarDescricao(string descricao)
        {
            Descricao = new DescricaoVO(descricao, 255);
            AtualizarTimestamp();
        }
    }
}
