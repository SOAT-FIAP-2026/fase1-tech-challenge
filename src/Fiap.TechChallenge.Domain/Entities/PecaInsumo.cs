namespace Fiap.TechChallenge.Domain.Entities
{
    public class PecaInsumo
    {
        public Guid Id { get; private set; }
        public string Descricao { get; private set; } = string.Empty;
        public decimal ValorUnitario { get; private set; }

        protected PecaInsumo() { }

        public PecaInsumo(string descricao, decimal valorUnitario)
        {
            Id = Guid.NewGuid();
            Descricao = descricao;
            ValorUnitario = valorUnitario;
        }
    }
}
