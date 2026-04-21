using Fiap.TechChallenge.Domain.ValueObjects;

namespace Fiap.TechChallenge.Domain.Tests.ValueObjects
{
    public class EmailUsuarioVOTests
    {
        [Fact]
        public void Construtor_EmailValido_DeveCriarObjeto()
        {
            // 1. Arrange (Preparar)
            string emailValido = "diogenes@friboi.com.br";

            // 2. Act (Agir)
            var emailVO = new EmailUsuarioVO(emailValido);

            // 3. Assert (Verificar)
            Assert.Equal(emailValido, emailVO.Endereco);
        }

        [Fact]
        public void Construtor_EmailInvalido_DeveLancarExcecao()
        {
            // 1. Arrange
            string emailInvalido = "diogenes@friboi"; 

            // 2 e 3. Act & Assert juntos
            Assert.Throws<ArgumentException>(() => new EmailUsuarioVO(emailInvalido));
        }

        [Fact]
        public void Construtor_EmailVazio_DeveLancarExcecao()
        {
            // 1. Arrange
            string emailVazio = "";

            // 2 e 3. Act & Assert
            Assert.Throws<ArgumentException>(() => new EmailUsuarioVO(emailVazio));
        }

        [Fact]
        public void Construtor_EmailGrandeDemais_DeveLancarExcecao()
        {
            // 1. Arrange
            string emailInvalido = "diogenes.kenedy.nascimento.oliveira.junior@friboi.com";

            // 2 e 3. Act & Assert juntos
            Assert.Throws<ArgumentException>(() => new EmailUsuarioVO(emailInvalido));
        }
    }
}