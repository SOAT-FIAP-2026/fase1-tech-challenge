using Fiap.TechChallenge.Application.DTOs.Requests;
using Fiap.TechChallenge.Application.Services;
using Fiap.TechChallenge.Domain.Entities;
using Fiap.TechChallenge.Domain.Interfaces.Repository;
using Fiap.TechChallenge.Domain.Interfaces.Security;
using Fiap.TechChallenge.Domain.Interfaces.Service;
using Fiap.TechChallenge.Domain.ValueObjects;
using Moq;

namespace Fiap.TechChallenge.Tests.Fiap.TechChallenge.Application.Services
{
    public class AutenticacaoServiceTests
    {
        private readonly Mock<IUsuarioRepository> _usuarioRepositoryMock;
        private readonly Mock<ITokenService> _tokenServiceMock;
        private readonly Mock<ICrypto> _cryptoServiceMock;
        private readonly AutenticacaoService _service;

        public AutenticacaoServiceTests()
        {
            _usuarioRepositoryMock = new Mock<IUsuarioRepository>();
            _tokenServiceMock = new Mock<ITokenService>();
            _cryptoServiceMock = new Mock<ICrypto>();
            _service = new AutenticacaoService(
                _usuarioRepositoryMock.Object,
                _tokenServiceMock.Object,
                _cryptoServiceMock.Object);
        }

        // ─── Cadastrar ───────────────────────────────────────────────────────────

        [Fact]
        public async Task Cadastrar_ComEmailDisponivel_DeveAdicionarUsuarioERetornarGuid()
        {
            // Arrange
            var request = new CadastrarRequest
            {
                Nome = "Joao Silva",
                Email = "joao@email.com",
                Login = "joao@email.com",
                Senha = "Senha@Forte1",
                IdPermissao = Guid.NewGuid()
            };

            _usuarioRepositoryMock
                .Setup(r => r.ExisteEmail(It.IsAny<EmailVO>()))
                .ReturnsAsync(false);

            _cryptoServiceMock
                .Setup(c => c.CriptografarSenha(It.IsAny<string>()))
                .Returns("hash_seguro");

            _usuarioRepositoryMock
                .Setup(r => r.Adicionar(It.IsAny<Usuario>()))
                .Returns(Task.CompletedTask);

            // Act
            Guid id = await _service.Cadastrar(request);

            // Assert
            Assert.NotEqual(Guid.Empty, id);
            _usuarioRepositoryMock.Verify(r => r.Adicionar(It.IsAny<Usuario>()), Times.Once);
        }

        [Fact]
        public async Task Cadastrar_ComEmailJaExistente_DeveLancarException()
        {
            // Arrange
            var request = new CadastrarRequest
            {
                Nome = "Joao Silva",
                Email = "joao@email.com",
                Login = "joao@email.com",
                Senha = "Senha@Forte1",
                IdPermissao = Guid.NewGuid()
            };

            _usuarioRepositoryMock
                .Setup(r => r.ExisteEmail(It.IsAny<EmailVO>()))
                .ReturnsAsync(true);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _service.Cadastrar(request));
            Assert.Equal("E-mail já cadastrado.", exception.Message);

            _usuarioRepositoryMock.Verify(r => r.Adicionar(It.IsAny<Usuario>()), Times.Never);
        }

        [Fact]
        public async Task Cadastrar_ComSenhaFraca_DeveLancarArgumentException()
        {
            // Arrange — senha sem maiúsculas, minúsculas, dígito e caractere especial
            var request = new CadastrarRequest
            {
                Nome = "Joao Silva",
                Email = "joao@email.com",
                Login = "joao@email.com",
                Senha = "senhafraca",
                IdPermissao = Guid.NewGuid()
            };

            _usuarioRepositoryMock
                .Setup(r => r.ExisteEmail(It.IsAny<EmailVO>()))
                .ReturnsAsync(false);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _service.Cadastrar(request));

            _cryptoServiceMock.Verify(c => c.CriptografarSenha(It.IsAny<string>()), Times.Never);
            _usuarioRepositoryMock.Verify(r => r.Adicionar(It.IsAny<Usuario>()), Times.Never);
        }

        [Fact]
        public async Task Cadastrar_ComEmailInvalido_DeveLancarArgumentException()
        {
            // Arrange
            var request = new CadastrarRequest
            {
                Nome = "Joao Silva",
                Email = "email-invalido-sem-arroba",
                Login = "email-invalido-sem-arroba",
                Senha = "Senha@Forte1",
                IdPermissao = Guid.NewGuid()
            };

            // Act & Assert — EmailVO lança antes mesmo de chamar o repositório
            await Assert.ThrowsAsync<ArgumentException>(() => _service.Cadastrar(request));

            _usuarioRepositoryMock.Verify(r => r.ExisteEmail(It.IsAny<EmailVO>()), Times.Never);
            _usuarioRepositoryMock.Verify(r => r.Adicionar(It.IsAny<Usuario>()), Times.Never);
        }

        [Fact]
        public async Task Cadastrar_ComSenhaVazia_DeveLancarArgumentException()
        {
            // Arrange
            var request = new CadastrarRequest
            {
                Nome = "Joao Silva",
                Email = "joao@email.com",
                Login = "joao@email.com",
                Senha = "",
                IdPermissao = Guid.NewGuid()
            };

            _usuarioRepositoryMock
                .Setup(r => r.ExisteEmail(It.IsAny<EmailVO>()))
                .ReturnsAsync(false);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _service.Cadastrar(request));

            _cryptoServiceMock.Verify(c => c.CriptografarSenha(It.IsAny<string>()), Times.Never);
        }

        // ─── Login ───────────────────────────────────────────────────────────────

        [Fact]
        public async Task Login_ComCredenciaisValidas_DeveRetornarLoginResponseComTokenENome()
        {
            // Arrange
            const string senhaHash = "hash_bcrypt";
            const string tokenEsperado = "jwt.token.gerado";
            const string nomeEsperado = "Joao Silva";

            var usuario = new Usuario(
                nomeEsperado,
                "joao@email.com",
                new SenhaUsuarioVO(senhaHash),
                Guid.NewGuid());

            var request = new LoginRequest
            {
                Login = "joao@email.com",
                Senha = "Senha@Forte1"
            };

            _usuarioRepositoryMock
                .Setup(r => r.ObterPorLogin(It.IsAny<EmailVO>()))
                .ReturnsAsync(usuario);

            _cryptoServiceMock
                .Setup(c => c.VerificarSenha(request.Senha, senhaHash))
                .Returns(true);

            _tokenServiceMock
                .Setup(t => t.GerarToken(usuario))
                .Returns(tokenEsperado);

            // Act
            var response = await _service.Login(request);

            // Assert
            Assert.NotNull(response);
            Assert.Equal(tokenEsperado, response.Token);
            Assert.Equal(nomeEsperado, response.NomeUsuario);
        }

        [Fact]
        public async Task Login_ComUsuarioNaoEncontrado_DeveLancarUnauthorizedAccessException()
        {
            // Arrange
            var request = new LoginRequest
            {
                Login = "naoexiste@email.com",
                Senha = "Senha@Forte1"
            };

            _usuarioRepositoryMock
                .Setup(r => r.ObterPorLogin(It.IsAny<EmailVO>()))
                .ReturnsAsync((Usuario?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _service.Login(request));

            Assert.Equal("Usuário ou senha inválidos.", exception.Message);

            _tokenServiceMock.Verify(t => t.GerarToken(It.IsAny<Usuario>()), Times.Never);
        }

        [Fact]
        public async Task Login_ComSenhaIncorreta_DeveLancarUnauthorizedAccessException()
        {
            // Arrange
            const string senhaHash = "hash_bcrypt";

            var usuario = new Usuario(
                "Joao Silva",
                "joao@email.com",
                new SenhaUsuarioVO(senhaHash),
                Guid.NewGuid());

            var request = new LoginRequest
            {
                Login = "joao@email.com",
                Senha = "SenhaErrada@1"
            };

            _usuarioRepositoryMock
                .Setup(r => r.ObterPorLogin(It.IsAny<EmailVO>()))
                .ReturnsAsync(usuario);

            _cryptoServiceMock
                .Setup(c => c.VerificarSenha(request.Senha, senhaHash))
                .Returns(false);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _service.Login(request));

            Assert.Equal("Usuário ou senha inválidos.", exception.Message);

            _tokenServiceMock.Verify(t => t.GerarToken(It.IsAny<Usuario>()), Times.Never);
        }

        [Fact]
        public async Task Login_ComEmailInvalido_DeveLancarArgumentException()
        {
            // Arrange — EmailVO lança antes mesmo de consultar o repositório
            var request = new LoginRequest
            {
                Login = "nao-e-um-email",
                Senha = "Senha@Forte1"
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _service.Login(request));

            _usuarioRepositoryMock.Verify(r => r.ObterPorLogin(It.IsAny<EmailVO>()), Times.Never);
        }
    }
}
