using Fiap.TechChallenge.Domain.Interfaces.Security;
using Fiap.TechChallenge.Domain.ValueObjects;
using Moq;

namespace Fiap.TechChallenge.Domain.Tests.ValueObjects
{
    public class NomeUsuarioVOTests
    {
        [Fact]
        public void Construtor_NomeValido_DeveCriarObjeto()
        {
            // 1. Arrange (Preparar)
            string nomeValido = "Junior";

            // 2. Act (Agir)
            NomeUsuarioVO nomeUsuarioVO = new NomeUsuarioVO(nomeValido);

            // 3. Assert (Verificar)
            Assert.Equal(nomeValido, nomeUsuarioVO.Valor);
        }

        [Fact]
        public void Construtor_NomeMenorQue3Caracteres_DeveLancarExcecao()
        {
            // 1. Arrange
            string nomeInvalido = "Ju";

            // 2 e 3. Act & Assert
            Assert.Throws<ArgumentException>(() => new NomeUsuarioVO(nomeInvalido));
        }

        [Fact]
        public void Construtor_NomeVazio_DeveLancarExcecao()
        {
            // 1. Arrange
            string nomeVazio = "";

            // 2 e 3. Act & Assert
            Assert.Throws<ArgumentException>(() => new NomeUsuarioVO(nomeVazio));
        }

        [Fact]
        public void Construtor_NomeGrandeDemais_DeveLancarExcecao()
        {
            // 1. Arrange
            string nomeInvalido = "diogenes kenedy nascimento oliveira junior friboi com123";

            // 2 e 3. Act & Assert
            Assert.Throws<ArgumentException>(() => new NomeUsuarioVO(nomeInvalido));
        }
    }
}