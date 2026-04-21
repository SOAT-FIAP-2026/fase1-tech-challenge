using System;
using System.Text.RegularExpressions;

namespace Fiap.TechChallenge.Domain.ValueObjects
{
    public record EmailUsuarioVO
    {
        public string Endereco { get; }

        private static readonly Regex EmailRegex = new Regex(
            @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly int TAMANHO_MAXIMO = 50;

        public EmailUsuarioVO(string endereco)
        {
            if (string.IsNullOrWhiteSpace(endereco))
                throw new ArgumentException("O e-mail não pode ser vazio.");

            string emailTratado = endereco.Trim().ToLower();

            if (!EmailRegex.IsMatch(emailTratado))
                throw new ArgumentException("O formato do e-mail é inválido.");

            if (emailTratado.Count() > TAMANHO_MAXIMO)
                throw new ArgumentException("O e-mail possui tamanho superior ao máximo permitido: " + TAMANHO_MAXIMO);

            Endereco = emailTratado;
        }
    }
}