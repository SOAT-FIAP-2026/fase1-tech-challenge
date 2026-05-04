using Fiap.TechChallenge.Domain.ValueObjects;

namespace Fiap.TechChallenge.Tests.Domain.ValueObjects
{
    public class CodigoVOTests
    {
        [Fact]
        public void Construtor_ValorValido_DeveNormalizarCodigo()
        {
            var codigo = new CodigoVO(" abc ");

            Assert.Equal("ABC", codigo.Valor);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void Construtor_ValorVazio_DeveLancarExcecao(string valor)
        {
            Assert.Throws<ArgumentException>(() => new CodigoVO(valor));
        }

        [Fact]
        public void Construtor_ValorComMenosDeTresCaracteres_DeveLancarExcecao()
        {
            Assert.Throws<ArgumentException>(() => new CodigoVO("AB"));
        }

        [Fact]
        public void Construtor_ValorComMaisDeDuzentosECinquentaECincoCaracteres_DeveLancarExcecao()
        {
            string valor = new('A', 256);

            Assert.Throws<ArgumentException>(() => new CodigoVO(valor));
        }
    }
}
