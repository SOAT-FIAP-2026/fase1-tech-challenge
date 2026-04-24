using Fiap.TechChallenge.Domain.ValueObjects;

namespace Fiap.TechChallenge.Domain.Entities
{
    public class StatusOrdemServico : EntidadeBase
    {
        public DescricaoVO Descricao { get; private set; } = null!;

        protected StatusOrdemServico() { }

        public StatusOrdemServico(string descricao) : base()
        {
            Descricao = new DescricaoVO(descricao, 100);
        }
    }
}
