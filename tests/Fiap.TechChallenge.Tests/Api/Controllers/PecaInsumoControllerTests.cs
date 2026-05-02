using Fiap.TechChallenge.Api.Controllers.V1;
using Fiap.TechChallenge.Application.DTOs.Requests;
using Fiap.TechChallenge.Application.DTOs.Responses;
using Fiap.TechChallenge.Application.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Fiap.TechChallenge.Tests.Fiap.TechChallenge.Api.Controllers
{
    public class PecaInsumoControllerTests
    {
        private readonly Mock<IPecaInsumoService> _pecaInsumoServiceMock;
        private readonly PecaInsumoController _controller;

        public PecaInsumoControllerTests()
        {
            _pecaInsumoServiceMock = new Mock<IPecaInsumoService>();
            _controller = new PecaInsumoController(_pecaInsumoServiceMock.Object);
        }

        [Fact]
        public async Task Criar_DeveRetornarIdDaPecaInsumo_QuandoRequestValido()
        {
            // Arrange
            PecaInsumoRequest request = new PecaInsumoRequest
            {
                Descricao = "Peça A",
                ValorUnitario = 10
            };

            Guid idEsperado = Guid.NewGuid();

            _pecaInsumoServiceMock
                .Setup(s => s.Criar(It.IsAny<PecaInsumoRequest>()))
                .ReturnsAsync(idEsperado);

            // Act
            var result = await _controller.Criar(request);

            // Assert
            result.Should().BeOfType<CreatedResult>();

            var createdResult = result.As<CreatedResult>();
            createdResult.Value.Should().Be(idEsperado);
        }

        [Fact]
        public async Task ObterPorId_DeveRetornarPecaInsumo_QuandoRequestValido()
        {
            // Arrange
            Guid idPecaInsumo = Guid.NewGuid();
            PecaInsumoResponse pecaInsumoEsperada = new PecaInsumoResponse
            (
                idPecaInsumo,
                "Peça A",
                10
            );

            _pecaInsumoServiceMock
                .Setup(s => s.ObterPorId(It.IsAny<Guid>()))
                .ReturnsAsync(pecaInsumoEsperada);

            // Act
            var result = await _controller.ObterPorId(idPecaInsumo);

            // Assert
            result.Should().BeOfType<OkObjectResult>();

            var okResult = result.As<OkObjectResult>();
            okResult.Value.Should().Be(pecaInsumoEsperada);
        }

        [Fact]
        public async Task ObterPorDescricao_DeveRetornarPecaInsumo_QuandoRequestValido()
        {
            // Arrange
            string descricao = "Peça A";
            PecaInsumoResponse pecaInsumoEsperada = new PecaInsumoResponse
            (
                Guid.NewGuid(),
                descricao,
                10
            );

            _pecaInsumoServiceMock
                .Setup(s => s.ObterPorDescricao(It.IsAny<string>()))
                .ReturnsAsync(pecaInsumoEsperada);

            // Act
            var result = await _controller.ObterPorDescricao(descricao);

            // Assert
            result.Should().BeOfType<OkObjectResult>();

            var okResult = result.As<OkObjectResult>();
            okResult.Value.Should().Be(pecaInsumoEsperada);
        }

        [Fact]
        public async Task ObterTodos_DeveRetornarTodasPecasInsumo_QuandoRequestValido()
        {
            // Arrange
            PecaInsumoResponse[] pecasInsumoEsperadas = new PecaInsumoResponse[]
            {
                new PecaInsumoResponse(Guid.NewGuid(), "Peça A", 10),
                new PecaInsumoResponse(Guid.NewGuid(), "Peça B", 20)
            };

            _pecaInsumoServiceMock
                .Setup(s => s.ObterTodos())
                .ReturnsAsync(pecasInsumoEsperadas);

            // Act
            var result = await _controller.ObterTodos();

            // Assert
            result.Should().BeOfType<OkObjectResult>();

            var okResult = result.As<OkObjectResult>();
            okResult.Value.Should().Be(pecasInsumoEsperadas);
        }

        [Fact]
        public async Task Atualizar_DeveRetornarMensagemPositiva_QuandoRequestValido()
        {
            // Arrange
            Guid idPecaInsumo = Guid.NewGuid();
            PecaInsumoRequest request = new PecaInsumoRequest
            { 
                Descricao = "Peça A",
                ValorUnitario = 10
            };

            PecaInsumoResponse pecasInsumoEsperadas = new PecaInsumoResponse
            (
                Guid.NewGuid(), 
                "Peça A", 
                10
            );

            _pecaInsumoServiceMock
                .Setup(s => s.Atualizar(It.IsAny<Guid>(), It.IsAny<PecaInsumoRequest>()))
                .ReturnsAsync(pecasInsumoEsperadas);

            // Act
            var result = await _controller.Atualizar(idPecaInsumo, request);

            // Assert
            result.Should().BeOfType<OkObjectResult>();

            var okResult = result.As<OkObjectResult>();
            okResult.Value.Should().Be(pecasInsumoEsperadas);
        }

        [Fact]
        public async Task Deletar_DeveRetornarNoContent_QuandoRequestValido()
        {
            // Arrange
            Guid idPecaInsumo = Guid.NewGuid();

            _pecaInsumoServiceMock
                .Setup(s => s.Deletar(It.IsAny<Guid>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Deletar(idPecaInsumo);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }
    }
}
