namespace Fiap.TechChallenge.Domain.Entities
{
    public class Estoque : EntidadeAuditavel
    {
        public Guid IdPecaInsumo { get; private set; }
        public int Quantidade { get; private set; }
        public PecaInsumo PecaInsumo { get; private set; } = null!;

        protected Estoque() { }

        public Estoque(Guid idPecaInsumo, int quantidade) : base()
        {
            if (quantidade < 0)
                throw new ArgumentException("A quantidade não pode ser negativa.");

            IdPecaInsumo = idPecaInsumo;
            Quantidade = quantidade;
        }

        public void AdicionarQuantidade(int quantidade)
        {
            if (quantidade <= 0)
                throw new ArgumentException("A quantidade a adicionar deve ser positiva.");

            Quantidade += quantidade;
            AtualizarTimestamp();
        }

        public void RemoverQuantidade(int quantidade)
        {
            if (quantidade <= 0)
                throw new ArgumentException("A quantidade a remover deve ser positiva.");

            if (Quantidade - quantidade < 0)
                throw new InvalidOperationException("Estoque insuficiente.");

            Quantidade -= quantidade;
            AtualizarTimestamp();
        }
    }
}
