using Fiap.TechChallenge.Domain.Interfaces.Security;
using Fiap.TechChallenge.Domain.ValueObjects;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fiap.TechChallenge.Domain.Entities
{
    public class UsuarioTests
    {
        [Fact]
        public void Construtor_UsuarioValido_DeveCriarObjeto()
        {
            // 1. Arrange (Preparar)
            string nomeValido = "Diogenes";
            string emailValido = "diogenes@friboi.com.br";
            string senhaValida = "Junior123@";
            string hashEsperado = "$2a$11$codigo_embaralhado_falso";

            var cryptoMock = new Mock<ICrypto>();
            cryptoMock
            .Setup(c => c.CriptografarSenha(senhaValida))
            .Returns(hashEsperado);

            SenhaUsuarioVO senhaVO = SenhaUsuarioVO.CriarNova(senhaValida, cryptoMock.Object);

            // 2. Act (Agir)
            var idPermissao = Guid.NewGuid();
            var usuario = new Usuario(nomeValido, emailValido, senhaVO, idPermissao);

            // 3. Assert (Verificar)
            Assert.NotEqual(Guid.Empty, usuario.Id);

            Assert.Equal(nomeValido, usuario.Nome.Valor);
            Assert.Equal(emailValido, usuario.Email.Endereco);

            Assert.Equal(hashEsperado, usuario.Senha.Hash);
            Assert.Equal(idPermissao, usuario.IdPermissao);
        }
        
        [Fact]
        public void Construtor_UsuarioValido_DeveAlterarNome()
        {
            // 1. Arrange (Preparar)
            string nomeValido = "Diogenes";
            string emailValido = "diogenes@friboi.com.br";
            string senhaValida = "Junior123@";
            string novoNome = "Kenedy";

            var cryptoMock = new Mock<ICrypto>();

            SenhaUsuarioVO senhaVO = SenhaUsuarioVO.CriarNova(senhaValida, cryptoMock.Object);

            // 2. Act (Agir)
            var usuario = new Usuario(nomeValido, emailValido, senhaVO, Guid.NewGuid());

            usuario.AlterarNome(novoNome);

            // 3. Assert (Verificar)
            Assert.Equal(usuario.Nome.Valor, novoNome);
        }
    }
}