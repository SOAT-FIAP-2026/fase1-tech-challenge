namespace Fiap.TechChallenge.Domain.ValueObjects
{
    public record ValorMonetarioVO
    {
        public decimal Valor { get; }

        protected ValorMonetarioVO() { }

        public ValorMonetarioVO(decimal valor)
        {
            if (valor < 0)
                throw new ArgumentException("O valor monetário não pode ser negativo.");

            Valor = Math.Round(valor, 2);
        }
    }
}
