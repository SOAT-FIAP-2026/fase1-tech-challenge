using Fiap.TechChallenge.Domain.ValueObjects;

namespace Fiap.TechChallenge.Domain.Tests.ValueObjects
{
    public class AnoVeiculoVOTests
    {
        [Fact]
        public void Construtor_ComAnoValido_DeveCriarComSucesso()
        {
            var ano = new AnoVeiculoVO(2020);

            Assert.Equal(2020, ano.Valor);
        }

        [Fact]
        public void Construtor_ComAno1900_DeveCriarComSucesso()
        {
            var ano = new AnoVeiculoVO(1900);

            Assert.Equal(1900, ano.Valor);
        }

        [Fact]
        public void Construtor_ComAnoAtualMaisUm_DeveCriarComSucesso()
        {
            int anoProximo = DateTime.UtcNow.Year + 1;

            var ano = new AnoVeiculoVO(anoProximo);

            Assert.Equal(anoProximo, ano.Valor);
        }

        [Fact]
        public void Construtor_ComAnoMenorQue1900_DeveLancarExcecao()
        {
            var excecao = Assert.Throws<ArgumentException>(() => new AnoVeiculoVO(1899));

            Assert.Contains("1900", excecao.Message);
        }

        [Fact]
        public void Construtor_ComAnoMaiorQueProximoAno_DeveLancarExcecao()
        {
            int anoInvalido = DateTime.UtcNow.Year + 2;

            var excecao = Assert.Throws<ArgumentException>(() => new AnoVeiculoVO(anoInvalido));

            Assert.Contains("posterior", excecao.Message);
        }

        [Theory]
        [InlineData(1950)]
        [InlineData(2000)]
        [InlineData(2010)]
        [InlineData(2024)]
        public void Construtor_ComAnosValidos_DeveCriarComSucesso(int anoValido)
        {
            var ano = new AnoVeiculoVO(anoValido);

            Assert.Equal(anoValido, ano.Valor);
        }

        [Fact]
        public void Igualdade_AnosIguais_DeveSerVerdadeiro()
        {
            var ano1 = new AnoVeiculoVO(2020);
            var ano2 = new AnoVeiculoVO(2020);

            Assert.Equal(ano1, ano2);
        }

        [Fact]
        public void Igualdade_AnosDiferentes_DeveSerFalso()
        {
            var ano1 = new AnoVeiculoVO(2020);
            var ano2 = new AnoVeiculoVO(2021);

            Assert.NotEqual(ano1, ano2);
        }
    }
}
