using Fiap.TechChallenge.Domain.ValueObjects;

namespace Fiap.TechChallenge.Domain.Entities
{
    public class Servico : EntidadeAuditavel
    {
        public NomeVO Nome { get; private set; } = null!;
        public DescricaoVO Descricao { get; private set; } = null!;
        public ValorMonetarioVO ValorUnitario { get; private set; } = null!;
        public int TempoEstimadoMinutos { get; private set; }

        protected Servico() { }

        public Servico(string nome, string descricao, decimal valorUnitario, int tempoEstimadoMinutos = 60) : base()
        {
            Nome = new NomeVO(nome);
            Descricao = new DescricaoVO(descricao, 255);
            ValorUnitario = new ValorMonetarioVO(valorUnitario);
            DefinirTempoEstimado(tempoEstimadoMinutos);
        }

        public void Atualizar(string nome, string descricao, decimal valorUnitario, int tempoEstimadoMinutos)
        {
            Nome = new NomeVO(nome);
            Descricao = new DescricaoVO(descricao, 255);
            ValorUnitario = new ValorMonetarioVO(valorUnitario);
            DefinirTempoEstimado(tempoEstimadoMinutos);
            AtualizarTimestamp();
        }

        private void DefinirTempoEstimado(int tempoEstimadoMinutos)
        {
            if (tempoEstimadoMinutos <= 0)
                throw new ArgumentException("O tempo estimado deve ser maior que zero.", nameof(tempoEstimadoMinutos));

            TempoEstimadoMinutos = tempoEstimadoMinutos;
        }
    }
}
