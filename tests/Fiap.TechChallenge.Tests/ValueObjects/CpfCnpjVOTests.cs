using Fiap.TechChallenge.Domain.ValueObjects;

namespace Fiap.TechChallenge.Domain.Tests.ValueObjects
{
    public class CpfCnpjVOTests
    {
        [Fact]
        public void AceitaCpfValido()
        {
            string cpf = GenerateValidCpf();
            var vo = new CpfCnpjVO(cpf);
            Assert.Equal(11, vo.Valor.Length);
        }

        [Fact]
        public void AceitaCnpjValido()
        {
            string cnpj = GenerateValidCnpj();
            var vo = new CpfCnpjVO(cnpj);
            Assert.Equal(14, vo.Valor.Length);
        }

        [Fact]
        public void LancaParaValoresInvalidos()
        {
            Assert.Throws<ArgumentException>(() => new CpfCnpjVO("11111111111"));
            Assert.Throws<ArgumentException>(() => new CpfCnpjVO("123"));
        }

        // Helpers para gerar CPF/CNPJ válidos
        private static string GenerateValidCpf()
        {
            var rnd = new Random(42);
            int[] nums = new int[9];
            for (int i = 0; i < 9; i++) nums[i] = rnd.Next(0, 10);
            if (nums.Distinct().Count() == 1) nums[0] = (nums[0] + 1) % 10;

            int soma = 0;
            for (int i = 0; i < 9; i++) soma += nums[i] * (10 - i);
            int resto = soma % 11;
            int dig1 = resto < 2 ? 0 : 11 - resto;

            soma = 0;
            for (int i = 0; i < 9; i++) soma += nums[i] * (11 - i);
            soma += dig1 * 2;
            resto = soma % 11;
            int dig2 = resto < 2 ? 0 : 11 - resto;

            string base9 = string.Concat(nums.Select(d => d.ToString()));
            return base9 + dig1 + dig2;
        }

        private static string GenerateValidCnpj()
        {
            var rnd = new Random(1337);
            int[] nums = new int[12];
            for (int i = 0; i < 12; i++) nums[i] = rnd.Next(0, 10);
            if (nums.Distinct().Count() == 1) nums[0] = (nums[0] + 1) % 10;

            int[] mult1 = {5,4,3,2,9,8,7,6,5,4,3,2};
            int soma = 0;
            for (int i = 0; i < 12; i++) soma += nums[i] * mult1[i];
            int resto = soma % 11;
            int dig1 = resto < 2 ? 0 : 11 - resto;

            int[] mult2 = {6,5,4,3,2,9,8,7,6,5,4,3,2};
            soma = 0;
            for (int i = 0; i < 12; i++) soma += nums[i] * mult2[i];
            soma += dig1 * mult2[12];
            resto = soma % 11;
            int dig2 = resto < 2 ? 0 : 11 - resto;

            string base12 = string.Concat(nums.Select(d => d.ToString()));
            return base12 + dig1 + dig2;
        }
    }
}
