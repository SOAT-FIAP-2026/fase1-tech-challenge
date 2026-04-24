using System.Text.RegularExpressions;

namespace Fiap.TechChallenge.Domain.ValueObjects
{
    public record CelularVO
    {
        public string Numero { get; }

        private static readonly Regex ApenasDigitosRegex = new(@"\D", RegexOptions.Compiled);
        private static readonly Regex CelularBrRegex = new(@"^\d{10,11}$", RegexOptions.Compiled);

        protected CelularVO() { }

        public CelularVO(string numero)
        {
            if (string.IsNullOrWhiteSpace(numero))
                throw new ArgumentException("O celular não pode ser vazio.");

            string limpo = ApenasDigitosRegex.Replace(numero, "");

            if (!CelularBrRegex.IsMatch(limpo))
                throw new ArgumentException("O celular deve ter 10 ou 11 dígitos (DDD + número).");

            Numero = limpo;
        }
    }
}
