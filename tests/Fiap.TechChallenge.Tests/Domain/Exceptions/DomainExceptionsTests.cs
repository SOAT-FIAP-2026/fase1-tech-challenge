using Fiap.TechChallenge.Domain.Exceptions;

namespace Fiap.TechChallenge.Tests.Domain.Exceptions
{
    public class DomainExceptionsTests
    {
        [Fact]
        public void UsuarioNaoEncontradoException_DeveHerdaDomainExceptionEDefinirMensagem()
        {
            var login = "usuario.teste";

            var exception = new UsuarioNaoEncontradoException(login);

            Assert.IsAssignableFrom<DomainException>(exception);
            Assert.Equal($"Usuário com login '{login}' não encontrado.", exception.Message);
        }

        [Fact]
        public void CredenciaisInvalidasException_DeveHerdaDomainExceptionEDefinirMensagem()
        {
            var exception = new CredenciaisInvalidasException();

            Assert.IsAssignableFrom<DomainException>(exception);
            Assert.Equal("Usuário ou senha inválidos.", exception.Message);
        }

        [Fact]
        public void ErroGerarTokenException_DeveHerdaDomainExceptionEDefinirMensagem()
        {
            var exception = new ErroGerarTokenException();

            Assert.IsAssignableFrom<DomainException>(exception);
            Assert.Equal("Erro ao gerar token de autenticação.", exception.Message);
        }

        [Fact]
        public void OrdemServicoNaoEncontradaException_DeveHerdaDomainExceptionEDefinirMensagem()
        {
            var id = Guid.NewGuid();

            var exception = new OrdemServicoNaoEncontradaException(id);

            Assert.IsAssignableFrom<DomainException>(exception);
            Assert.Equal($"Ordem de serviço com id '{id}' não encontrada.", exception.Message);
        }

        [Fact]
        public void PecaInsumoNaoEncontradaException_DeveHerdaDomainExceptionEDefinirMensagem()
        {
            var id = Guid.NewGuid();

            var exception = new PecaInsumoNaoEncontradaException(id);

            Assert.IsAssignableFrom<DomainException>(exception);
            Assert.Equal($"Peça/Insumo com id '{id}' não encontrado.", exception.Message);
        }

        [Fact]
        public void StatusOrdemServicoNaoEncontradoException_DeveHerdaDomainExceptionEDefinirMensagem()
        {
            const string descricao = "Finalizada";

            var exception = new StatusOrdemServicoNaoEncontradoException(descricao);

            Assert.IsAssignableFrom<DomainException>(exception);
            Assert.Equal($"Status de ordem de serviço '{descricao}' não encontrado.", exception.Message);
        }
    }
}