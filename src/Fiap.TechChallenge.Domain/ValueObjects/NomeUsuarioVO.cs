namespace Fiap.TechChallenge.Domain.ValueObjects
{
    public record NomeUsuarioVO
    {
        public string Valor { get; }

        public NomeUsuarioVO(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
                throw new ArgumentException("O nome não pode ser vazio.");

            if (valor.Length < 3)
                throw new ArgumentException("O nome deve ter pelo menos 3 caracteres.");

            if (valor.Length > 50)
                throw new ArgumentException("O nome não pode exceder 50 caracteres.");

            Valor = valor.Trim();
        }
    }
}