namespace Fiap.TechChallenge.Domain.ValueObjects
{
    public record NomeVO
    {
        public string Valor { get; }

        protected NomeVO() { }

        public NomeVO(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
                throw new ArgumentException("O nome não pode ser vazio.");

            if (valor.Length < 3)
                throw new ArgumentException("O nome deve ter pelo menos 3 caracteres.");

            if (valor.Length > 255)
                throw new ArgumentException("O nome não pode exceder 255 caracteres.");

            Valor = valor.Trim();
        }
    }
}
