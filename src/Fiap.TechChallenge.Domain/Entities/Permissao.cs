using Fiap.TechChallenge.Domain.ValueObjects;

namespace Fiap.TechChallenge.Domain.Entities
{
    public class Permissao : EntidadeAuditavel
    {
        public DescricaoVO Descricao { get; private set; } = null!;

        protected Permissao() { }

        public Permissao(string descricao) : base()
        {
            Descricao = new DescricaoVO(descricao, 50);
        }

        public void AlterarDescricao(string descricao)
        {
            Descricao = new DescricaoVO(descricao, 50);
            AtualizarTimestamp();
        }
    }
}
