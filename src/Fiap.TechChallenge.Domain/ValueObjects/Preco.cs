namespace Fiap.TechChallenge.Domain.ValueObjects
{
    public record PrecoVO
    {
        public float Valor { get; }

        public PrecoVO(float valor)
        {
            if (valor < 0)
                throw new ArgumentException("O preço não pode ser negativo.");

            Valor = valor;
        }
    }
}