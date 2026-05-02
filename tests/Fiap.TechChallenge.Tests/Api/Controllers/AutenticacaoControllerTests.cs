using Fiap.TechChallenge.Api.Controllers.V1;
using Fiap.TechChallenge.Application.DTOs.Requests;
using Fiap.TechChallenge.Application.DTOs.Responses;
using Fiap.TechChallenge.Application.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Fiap.TechChallenge.Tests.Fiap.TechChallenge.Api.Controllers
{
    public class AutenticacaoControllerTests
    {
        private readonly Mock<IAutenticacaoService> _autenticacaoServiceMock;
        private readonly AutenticacaoController _controller;

        public AutenticacaoControllerTests()
        {
            _autenticacaoServiceMock = new Mock<IAutenticacaoService>();
            _controller = new AutenticacaoController(_autenticacaoServiceMock.Object);
        }

        [Fact]
        public async Task Cadastrar_DeveRetornarIdDoUsuario_QuandoRequestValido()
        {
            // Arrange
            CadastrarRequest request = new CadastrarRequest
            {
                Nome = "Joao Silva",
                Email = "joao@email.com",
                Login = "joaosilva",
                Senha = "Senha123!",
                IdPermissao = Guid.NewGuid()
            };

            Guid idEsperado = Guid.NewGuid();

            _autenticacaoServiceMock
                .Setup(s => s.Cadastrar(It.IsAny<CadastrarRequest>()))
                .ReturnsAsync(idEsperado);

            // Act
            var result = await _controller.Cadastrar(request);

            // Assert
            result.Should().BeOfType<CreatedResult>();

            var createdResult = result.As<CreatedResult>();
            createdResult.Value.Should().Be(idEsperado);
        }

        [Fact]
        public async Task Login_DeveRetornarObjetoDoUsuario_QuandoRequestValido()
        {
            // Arrange
            LoginRequest request = new LoginRequest
            {
                Login = "joaosilva",
                Senha = "Senha123!",
            };

            LoginResponse loginResponseEsperado = new LoginResponse("token123", "joaosilva");

            _autenticacaoServiceMock
                .Setup(s => s.Login(It.IsAny<LoginRequest>()))
                .ReturnsAsync(loginResponseEsperado);

            // Act
            var result = await _controller.Login(request);

            // Assert
            result.Should().BeOfType<OkObjectResult>();

            var okResult = result.As<OkObjectResult>();
            okResult.Value.Should().Be(loginResponseEsperado);
        }
    }
}
