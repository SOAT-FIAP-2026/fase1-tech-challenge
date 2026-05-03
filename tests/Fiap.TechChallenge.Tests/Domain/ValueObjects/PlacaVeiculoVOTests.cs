using System;
using Fiap.TechChallenge.Domain.ValueObjects;
using Xunit;

namespace Fiap.TechChallenge.Tests.Fiap.TechChallenge.Domain.ValueObjects
{
    public class PlacaVeiculoVOTests
    {
        [Fact]
        public void Construtor_NormalizaEAceitaFormatosValidos()
        {
            var p1 = new PlacaVeiculoVO("ABC-1234");
            Assert.Equal("ABC1234", p1.Valor);

            var p2 = new PlacaVeiculoVO("abc1d23");
            Assert.Equal("ABC1D23", p2.Valor);
        }

        [Fact]
        public void Construtor_LancaParaFormatoInvalido()
        {
            Assert.Throws<ArgumentException>(() => new PlacaVeiculoVO("1234ABC"));
            Assert.Throws<ArgumentException>(() => new PlacaVeiculoVO(""));
        }
    }
}
