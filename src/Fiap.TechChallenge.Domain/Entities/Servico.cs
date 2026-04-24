using Fiap.TechChallenge.Domain.ValueObjects;

namespace Fiap.TechChallenge.Domain.Entities
{
    public class Servico : EntidadeAuditavel
    {
        public NomeVO Nome { get; private set; } = null!;
        public DescricaoVO Descricao { get; private set; } = null!;
        public ValorMonetarioVO ValorUnitario { get; private set; } = null!;

        protected Servico() { }

        public Servico(string nome, string descricao, decimal valorUnitario) : base()
        {
            Nome = new NomeVO(nome);
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
