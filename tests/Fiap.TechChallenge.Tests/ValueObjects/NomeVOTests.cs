using Fiap.TechChallenge.Domain.ValueObjects;

namespace Fiap.TechChallenge.Domain.Tests.ValueObjects
{
    public class NomeVOTests
    {
        [Fact]
        public void Construtor_NomeValido_DeveCriarObjeto()
        {
            // 1. Arrange (Preparar)
            string nomeValido = "Junior";

            // 2. Act (Agir)
            NomeVO nomeVO = new(nomeValido);

            // 3. Assert (Verificar)
            Assert.Equal(nomeValido, nomeVO.Valor);
        }

        [Fact]
        public void Construtor_NomeMenorQue3Caracteres_DeveLancarExcecao()
        {
            // 1. Arrange
            string nomeInvalido = "Ju";

            // 2 e 3. Act & Assert
            Assert.Throws<ArgumentException>(() => new NomeVO(nomeInvalido));
        }

        [Fact]
        public void Construtor_NomeVazio_DeveLancarExcecao()
        {
            // 1. Arrange
            string nomeVazio = "";

            // 2 e 3. Act & Assert
            Assert.Throws<ArgumentException>(() => new NomeVO(nomeVazio));
        }

        [Fact]
        public void Construtor_NomeGrandeDemais_DeveLancarExcecao()
        {
            // 1. Arrange
            string nomeInvalido = new('a', 256);

            // 2 e 3. Act & Assert
            Assert.Throws<ArgumentException>(() => new NomeVO(nomeInvalido));
        }
    }
}
