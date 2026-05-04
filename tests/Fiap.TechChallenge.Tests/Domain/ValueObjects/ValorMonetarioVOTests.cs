using Fiap.TechChallenge.Domain.ValueObjects;

namespace Fiap.TechChallenge.Tests.Domain.ValueObjects
{
    public class ValorMonetarioVOTests
    {
        [Fact]
        public void Construtor_ComValorValido_DeveArredondarParaDuasCasas()
        {
            var valor = new ValorMonetarioVO(123.456m);

            Assert.Equal(123.46m, valor.Valor);
        }

        [Fact]
        public void Construtor_ComValorNegativo_DeveLancarExcecao()
        {
            var exception = Assert.Throws<ArgumentException>(() => new ValorMonetarioVO(-1m));

            Assert.Equal("O valor monetário não pode ser negativo.", exception.Message);
        }
    }
}