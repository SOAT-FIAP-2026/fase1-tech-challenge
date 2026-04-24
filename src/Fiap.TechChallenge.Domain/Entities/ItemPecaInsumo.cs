namespace Fiap.TechChallenge.Domain.Entities
{
    public class ItemPecaInsumo : EntidadeBase
    {
        public Guid IdOrdemServico { get; private set; }
        public Guid IdPecaInsumo { get; private set; }

        public OrdemServico OrdemServico { get; private set; } = null!;
        public PecaInsumo PecaInsumo { get; private set; } = null!;

        protected ItemPecaInsumo() { }

        public ItemPecaInsumo(Guid idOrdemServico, Guid idPecaInsumo) : base()
        {
            IdOrdemServico = idOrdemServico;
            IdPecaInsumo = idPecaInsumo;
        }
    }
}
