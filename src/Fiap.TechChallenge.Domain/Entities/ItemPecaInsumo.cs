namespace Fiap.TechChallenge.Domain.Entities
{
    public class ItemPecaInsumo
    {
        public Guid Id { get; private set; }
        public Guid IdOrdemServico { get; private set; }
        public Guid IdPecaInsumo { get; private set; }

        protected ItemPecaInsumo() { }

        public ItemPecaInsumo(Guid idOrdemServico, Guid idPecaInsumo)
        {
            Id = Guid.NewGuid();
            IdOrdemServico = idOrdemServico;
            IdPecaInsumo = idPecaInsumo;
        }
    }
}
