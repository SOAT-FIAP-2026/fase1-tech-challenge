using Fiap.TechChallenge.Domain.Interfaces.Security;
using Fiap.TechChallenge.Domain.ValueObjects;
using Moq;

namespace Fiap.TechChallenge.Tests.Fiap.TechChallenge.Domain.ValueObjects
{
    public class SenhaUsuarioVOTests
    {
        [Fact]
        public void Construtor_SenhaValido_DeveCriarObjeto()
        {
            // 1. Arrange (Preparar)
            string senhaValida = "Junior@123";
            string hashEsperado = "$2a$11$codigo_embaralhado_falso"; 

            // Criamos o Mock e ensinamos o que ele deve fazer
            var cryptoMock = new Mock<ICrypto>();
            cryptoMock
                .Setup(c => c.CriptografarSenha(senhaValida))
                .Returns(hashEsperado);

            // 2. Act (Agir)
            SenhaUsuarioVO senhaVO = SenhaUsuarioVO.CriarNova(senhaValida,
                cryptoMock.Object);

            // 3. Assert (Verificar)
            Assert.Equal(hashEsperado, senhaVO.Hash);
            Assert.NotEqual(senhaValida, senhaVO.Hash);

            // Verifica se a interface foi realmente chamada exatamente 1 vez
            cryptoMock.Verify(c => c.CriptografarSenha(senhaValida), Times.Once);
        }

        [Fact]
        public void Construtor_SenhaMenorQue8Caracteres_DeveLancarExcecao()
        {
            // 1. Arrange
            string senhaInvalida = "Jun@1";

            var cryptoMock = new Mock<ICrypto>();
            cryptoMock
                .Setup(c => c.CriptografarSenha(senhaInvalida))
                .Returns(String.Empty);

            // 2 e 3. Act & Assert
            Assert.Throws<ArgumentException>(() => SenhaUsuarioVO.CriarNova(senhaInvalida,
                cryptoMock.Object));
        }

        [Fact]
        public void Construtor_SenhaVazia_DeveLancarExcecao()
        {
            // 1. Arrange
            string senhaVazia = "";

            var cryptoMock = new Mock<ICrypto>();
            cryptoMock
                .Setup(c => c.CriptografarSenha(senhaVazia))
                .Returns(String.Empty);

            // 2 e 3. Act & Assert
            Assert.Throws<ArgumentException>(() => SenhaUsuarioVO.CriarNova(senhaVazia,
                cryptoMock.Object));
        }

        [Fact]
        public void Construtor_SenhaGrandeDemais_DeveLancarExcecao()
        {
            // 1. Arrange
            string senhaInvalida = "diogenes.kenedy.nascimento.oliveira.junior@indigo.com123";

            var cryptoMock = new Mock<ICrypto>();
            cryptoMock
                .Setup(c => c.CriptografarSenha(senhaInvalida))
                .Returns(String.Empty);

            // 2 e 3. Act & Assert
            Assert.Throws<ArgumentException>(() => SenhaUsuarioVO.CriarNova(senhaInvalida,
                cryptoMock.Object));
        }
    }
}