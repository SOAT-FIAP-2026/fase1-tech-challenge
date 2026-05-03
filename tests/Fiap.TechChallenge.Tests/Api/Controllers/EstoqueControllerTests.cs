using Fiap.TechChallenge.Api.Controllers.V1;
using Fiap.TechChallenge.Application.DTOs.Requests;
using Fiap.TechChallenge.Application.DTOs.Responses;
using Fiap.TechChallenge.Application.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Fiap.TechChallenge.Tests.Fiap.TechChallenge.Api.Controllers
{
    public class EstoqueControllerTests
    {
        private readonly Mock<IEstoqueService> _estoqueServiceMock;
        private readonly EstoqueController _controller;

        public EstoqueControllerTests()
        {
            _estoqueServiceMock = new Mock<IEstoqueService>();
            _controller = new EstoqueController(_estoqueServiceMock.Object);
        }

        [Fact]
        public async Task Criar_DeveRetornarIdDoEstoque_QuandoRequestValido()
        {
            // Arrange
            EstoqueRequest request = new EstoqueRequest
            {
                IdPecaInsumo = Guid.NewGuid(),
                Quantidade = 10
            };

            Guid idEsperado = Guid.NewGuid();

            _estoqueServiceMock
                .Setup(s => s.Criar(It.IsAny<EstoqueRequest>()))
                .ReturnsAsync(idEsperado);

            // Act
            var result = await _controller.Criar(request);

            // Assert
            result.Should().BeOfType<CreatedResult>();

            var createdResult = result.As<CreatedResult>();
            createdResult.Value.Should().Be(idEsperado);
        }

        [Fact]
        public async Task VerificarQuantidadePorIdPecaInsumo_DeveRetornarQuantiadeDoEstoque_QuandoRequestValido()
        {
            // Arrange
            Guid idPecaInsumo = Guid.NewGuid();
            int quantidadeEsperada = 10;

            _estoqueServiceMock
                .Setup(s => s.VerificarQuantidadePorIdPecaInsumo(It.IsAny<Guid>()))
                .ReturnsAsync(quantidadeEsperada);

            // Act
            var result = await _controller.VerificarQuantidadePorIdPecaInsumo(idPecaInsumo);

            // Assert
            result.Should().BeOfType<OkObjectResult>();

            var okResult = result.As<OkObjectResult>();
            okResult.Value.Should().Be(quantidadeEsperada);
        }

        [Fact]
        public async Task VerificarQuantidadePorDescricaoPeca_DeveRetornarQuantiadeDoEstoque_QuandoRequestValido()
        {
            // Arrange
            string descricao = "Peça A";
            int quantidadeEsperada = 10;

            _estoqueServiceMock
                .Setup(s => s.VerificarQuantidadePorDescricaoPeca(It.IsAny<string>()))
                .ReturnsAsync(quantidadeEsperada);

            // Act
            var result = await _controller.VerificarQuantidadePorDescricaoPeca(descricao);
            // Assert
            result.Should().BeOfType<OkObjectResult>();

            var okResult = result.As<OkObjectResult>();
            okResult.Value.Should().Be(quantidadeEsperada);
        }

        [Fact]
        public async Task AdicionarQuantidade_DeveRetornarMensagemPositiva_QuandoRequestValido()
        {
            // Arrange
            Guid idPecaInsumo = Guid.NewGuid();
            int quantidade = 10;

            _estoqueServiceMock
                .Setup(s => s.AdicionarQuantidade(It.IsAny<Guid>(), It.IsAny<int>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.AdicionarQuantidade(idPecaInsumo, quantidade);

            // Assert
            result.Should().BeOfType<OkObjectResult>();

            var okResult = result.As<OkObjectResult>();
            okResult.Value.Should().Be("Quantidade adicionada com sucesso.");
        }

        [Fact]
        public async Task RemoverQuantidade_DeveRetornarMensagemPositiva_QuandoRequestValido()
        {
            // Arrange
            Guid idPecaInsumo = Guid.NewGuid();
            int quantidade = 10;

            _estoqueServiceMock
                .Setup(s => s.RemoverQuantidade(It.IsAny<Guid>(), It.IsAny<int>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.RemoverQuantidade(idPecaInsumo, quantidade);

            // Assert
            result.Should().BeOfType<OkObjectResult>();

            var okResult = result.As<OkObjectResult>();
            okResult.Value.Should().Be("Quantidade removida com sucesso.");
        }

        [Fact]
        public async Task Deletar_DeveRetornarNoContent_QuandoRequestValido()
        {
            // Arrange
            Guid idEstoque = Guid.NewGuid();

            _estoqueServiceMock
                .Setup(s => s.Deletar(It.IsAny<Guid>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Deletar(idEstoque);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }
    }
}
