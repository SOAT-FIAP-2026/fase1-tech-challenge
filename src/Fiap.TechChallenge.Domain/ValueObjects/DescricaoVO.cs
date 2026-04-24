namespace Fiap.TechChallenge.Domain.ValueObjects
{
    public record DescricaoVO
    {
        public string Valor { get; }

        protected DescricaoVO() { }

        public DescricaoVO(string valor, int tamanhoMaximo = 255)
        {
            if (string.IsNullOrWhiteSpace(valor))
                throw new ArgumentException("A descrição não pode ser vazia.");

            if (valor.Length > tamanhoMaximo)
                throw new ArgumentException($"A descrição não pode exceder {tamanhoMaximo} caracteres.");

            Valor = valor.Trim();
        }
    }
}
