using System.Text.RegularExpressions;

namespace Fiap.TechChallenge.Domain.ValueObjects
{
    public record CpfCnpjVO
    {
        public string Valor { get; }

        private static readonly Regex ApenasDigitosRegex = new(@"\D", RegexOptions.Compiled);

        protected CpfCnpjVO() { }

        public CpfCnpjVO(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
                throw new ArgumentException("O CPF/CNPJ não pode ser vazio.");

            string limpo = ApenasDigitosRegex.Replace(valor, "");

            if (limpo.Length != 11 && limpo.Length != 14)
                throw new ArgumentException("O CPF deve ter 11 dígitos e o CNPJ deve ter 14 dígitos.");

            if (limpo.Length == 11 && !ValidarCpf(limpo))
                throw new ArgumentException("CPF inválido.");

            if (limpo.Length == 14 && !ValidarCnpj(limpo))
                throw new ArgumentException("CNPJ inválido.");

            Valor = limpo;
        }

        private static bool ValidarCpf(string cpf)
        {
            if (cpf.Distinct().Count() == 1) return false;

            int[] multiplicadores1 = { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicadores2 = { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

            string tempCpf = cpf[..9];
            int soma = tempCpf.Select((c, i) => (c - '0') * multiplicadores1[i]).Sum();
            int resto = soma % 11;
            int digito1 = resto < 2 ? 0 : 11 - resto;

            tempCpf += digito1;
            soma = tempCpf.Select((c, i) => (c - '0') * multiplicadores2[i]).Sum();
            resto = soma % 11;
            int digito2 = resto < 2 ? 0 : 11 - resto;

            return cpf.EndsWith($"{digito1}{digito2}");
        }

        private static bool ValidarCnpj(string cnpj)
        {
            if (cnpj.Distinct().Count() == 1) return false;

            int[] multiplicadores1 = { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicadores2 = { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

            string tempCnpj = cnpj[..12];
            int soma = tempCnpj.Select((c, i) => (c - '0') * multiplicadores1[i]).Sum();
            int resto = soma % 11;
            int digito1 = resto < 2 ? 0 : 11 - resto;

            tempCnpj += digito1;
            soma = tempCnpj.Select((c, i) => (c - '0') * multiplicadores2[i]).Sum();
            resto = soma % 11;
            int digito2 = resto < 2 ? 0 : 11 - resto;

            return cnpj.EndsWith($"{digito1}{digito2}");
        }
    }
}
