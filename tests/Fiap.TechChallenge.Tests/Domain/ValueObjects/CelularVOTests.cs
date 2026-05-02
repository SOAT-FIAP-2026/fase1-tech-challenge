using Fiap.TechChallenge.Domain.ValueObjects;

namespace Fiap.TechChallenge.Tests.Fiap.TechChallenge.Domain.ValueObjects
{
    public class CelularVOTests
    {
        [Fact]
        public void AceitaFormatosComDDD()
        {
            var v1 = new CelularVO("(11) 91234-5678");
            Assert.Equal("11912345678", v1.Numero);

            var v2 = new CelularVO("11912345678");
            Assert.Equal("11912345678", v2.Numero);
        }

        [Fact]
        public void LancaQuandoInvalido()
        {
            Assert.Throws<ArgumentException>(() => new CelularVO("123"));
            Assert.Throws<ArgumentException>(() => new CelularVO("abcdefg"));
        }
    }
}
