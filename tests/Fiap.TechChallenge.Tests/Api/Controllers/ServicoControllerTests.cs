using Fiap.TechChallenge.Api.Controllers.V1;
using Fiap.TechChallenge.Application.DTOs.Requests;
using Fiap.TechChallenge.Application.DTOs.Responses;
using Fiap.TechChallenge.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Fiap.TechChallenge.Tests.Fiap.TechChallenge.Api.Controllers
{
    public class ServicoControllerTests
    {
        private readonly Mock<IServicoService> _servicoServiceMock;
        private readonly ServicoController _controller;

        public ServicoControllerTests()
        {
            _servicoServiceMock = new Mock<IServicoService>();
            _controller = new ServicoController(_servicoServiceMock.Object);
        }

        [Fact]
        public async Task Criar_DeveRetornarStatus201ComIdNoBody()
        {
            var request = new ServicoRequest
            {
                Nome = "Pintura",
                Descricao = "Pintura automotiva",
                ValorUnitario = 1800m
            };
            Guid id = Guid.NewGuid();

            _servicoServiceMock
                .Setup(s => s.Criar(request))
                .ReturnsAsync(id);

            IActionResult result = await _controller.Criar(request);

            var created = Assert.IsType<ObjectResult>(result);
            Assert.Equal(201, created.StatusCode);

            Assert.NotNull(created.Value);
            var idProperty = created.Value!.GetType().GetProperty("id");
            Assert.NotNull(idProperty);
            Assert.Equal(id, idProperty!.GetValue(created.Value));
        }

        [Fact]
        public async Task ObterPorId_DeveRetornarOk()
        {
            Guid id = Guid.NewGuid();
            var response = new ServicoResponse(id, "Alinhamento", "Alinhamento completo", 100m);

            _servicoServiceMock
                .Setup(s => s.ObterPorId(id))
                .ReturnsAsync(response);

            IActionResult result = await _controller.ObterPorId(id);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(response, ok.Value);
        }

        [Fact]
        public async Task ObterTodos_DeveRetornarOk()
        {
            var response = new List<ServicoResponse>
            {
                new(Guid.NewGuid(), "Servico A", "Descricao A", 10m),
                new(Guid.NewGuid(), "Servico B", "Descricao B", 20m)
            };

            _servicoServiceMock
                .Setup(s => s.ObterTodos())
                .ReturnsAsync(response);

            IActionResult result = await _controller.ObterTodos();

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(response, ok.Value);
        }

        [Fact]
        public async Task Atualizar_DeveRetornarOk()
        {
            Guid id = Guid.NewGuid();
            var request = new ServicoRequest
            {
                Nome = "Servico Atualizado",
                Descricao = "Descricao Atualizada",
                ValorUnitario = 200m
            };
            var response = new ServicoResponse(id, request.Nome, request.Descricao, request.ValorUnitario);

            _servicoServiceMock
                .Setup(s => s.Atualizar(id, request))
                .ReturnsAsync(response);

            IActionResult result = await _controller.Atualizar(id, request);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(response, ok.Value);
        }

        [Fact]
        public async Task Deletar_DeveRetornarNoContent()
        {
            Guid id = Guid.NewGuid();

            IActionResult result = await _controller.Deletar(id);

            Assert.IsType<NoContentResult>(result);
            _servicoServiceMock.Verify(s => s.Deletar(id), Times.Once);
        }
    }
}
