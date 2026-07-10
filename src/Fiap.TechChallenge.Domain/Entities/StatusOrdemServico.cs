using Fiap.TechChallenge.Domain.ValueObjects;

namespace Fiap.TechChallenge.Domain.Entities
{
    public class StatusOrdemServico : EntidadeBase
    {
        public DescricaoVO Descricao { get; private set; } = null!;
        public CodigoVO Codigo { get; private set; } = null!;

        protected StatusOrdemServico() { }

        public StatusOrdemServico(string descricao, string codigo) : base()
        {
            Descricao = new DescricaoVO(descricao, 100);
            Codigo = new CodigoVO(codigo);
        }
    }

    public sealed record StatusOS
    {
        public string Codigo { get; }
        public string Descricao { get; }

        private StatusOS(string codigo, string descricao)
        {
            Codigo = codigo;
            Descricao = descricao;
        }

        public static readonly StatusOS Recebida = new("RECEBIDA", "Recebida");
        public static readonly StatusOS EmDiagnostico = new("EM_DIAGNOSTICO", "Em Diagnostico");
        public static readonly StatusOS AguardandoAprovacao = new("AGUARDANDO_APROVACAO", "Aguardando aprovacao");
        public static readonly StatusOS OrcamentoAprovado = new("ORCAMENTO_APROVADO", "Orcamento Aprovado");
        public static readonly StatusOS OrcamentoReprovado = new("ORCAMENTO_REPROVADO", "Orcamento Reprovado");
        public static readonly StatusOS EmExecucao = new("EM_EXECUCAO", "Em Execucao");
        public static readonly StatusOS Finalizada = new("FINALIZADA", "Finalizada");
        public static readonly StatusOS Entregue = new("ENTREGUE", "Entregue");
        public static readonly StatusOS Cancelada = new("CANCELADA", "Cancelada");

    }
}
