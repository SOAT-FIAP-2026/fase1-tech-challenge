using System.Text.RegularExpressions;

namespace Fiap.TechChallenge.Domain.ValueObjects
{
    public record EmailVO
    {
        public string Endereco { get; }

        private static readonly Regex EmailRegex = new(
            @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        protected EmailVO() { }

        public EmailVO(string endereco)
        {
            if (string.IsNullOrWhiteSpace(endereco))
                throw new ArgumentException("O e-mail não pode ser vazio.");

            string emailTratado = endereco.Trim().ToLower();

            if (!EmailRegex.IsMatch(emailTratado))
                throw new ArgumentException("O formato do e-mail é inválido.");

            if (emailTratado.Length > 255)
                throw new ArgumentException("O e-mail possui tamanho superior ao máximo permitido: 255.");

            Endereco = emailTratado;
        }
    }
}
