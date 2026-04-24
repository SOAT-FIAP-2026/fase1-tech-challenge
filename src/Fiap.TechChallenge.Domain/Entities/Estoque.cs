namespace Fiap.TechChallenge.Domain.Entities
{
    public class Estoque
    {
        public Guid Id { get; private set; }
        public Guid IdPecaInsumo { get; private set; }
        public int Quantidade { get; private set; }

        protected Estoque() { }

        public Estoque(Guid idPecaInsumo, int quantidade)
        {
            Id = Guid.NewGuid();
            IdPecaInsumo = idPecaInsumo;
            Quantidade = quantidade;
        }
    }
}
