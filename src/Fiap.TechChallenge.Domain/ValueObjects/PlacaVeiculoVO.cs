using System.Text.RegularExpressions;

namespace Fiap.TechChallenge.Domain.ValueObjects
{
    public record PlacaVeiculoVO
    {
        public string Valor { get; }

        private static readonly Regex PlacaRegex = new(
            @"^[A-Z]{3}\d[A-Z0-9]\d{2}$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        protected PlacaVeiculoVO() { }

        public PlacaVeiculoVO(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
                throw new ArgumentException("A placa não pode ser vazia.");

            string limpa = valor.Trim().ToUpper().Replace("-", "");

            if (!PlacaRegex.IsMatch(limpa))
                throw new ArgumentException("Formato de placa inválido. Use o formato ABC1234 ou ABC1D23 (Mercosul).");

            Valor = limpa;
        }
    }
}
