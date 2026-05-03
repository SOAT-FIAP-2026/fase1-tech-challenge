namespace Fiap.TechChallenge.Domain.ValueObjects
{
    public record CodigoVO
    {
        public string Valor { get; }

    
        public CodigoVO(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
                throw new ArgumentException("O código não pode ser vazio.");

            if (valor.Length < 3)
                throw new ArgumentException("O código deve ter pelo menos 3 caracteres.");

            if (valor.Length > 255)
                throw new ArgumentException("O código não pode exceder 255 caracteres.");

            Valor = valor.Trim().ToUpper();
        }
    }
}
