using Fiap.TechChallenge.Api.Controllers.V1;
using Fiap.TechChallenge.Application.DTOs.Requests;
using Fiap.TechChallenge.Application.DTOs.Responses;
using Fiap.TechChallenge.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Fiap.TechChallenge.Tests.Controllers
{
    public class ClienteControllerTests
    {
        private readonly Mock<IClienteService> _clienteServiceMock;
        private readonly ClienteController _controller;

        public ClienteControllerTests()
        {
            _clienteServiceMock = new Mock<IClienteService>();
            _controller = new ClienteController(_clienteServiceMock.Object);
        }

        [Fact]
        public async Task Criar_DeveRetornarStatus201ComIdNoBody()
        {
            var request = new ClienteRequest
            {
                Nome = "Joao Silva",
                CpfCnpj = "52998224725",
                Email = "joao@email.com",
                Celular = "11999999999"
            };
            Guid id = Guid.NewGuid();

            _clienteServiceMock
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
            var response = new ClienteResponse(id, "Joao Silva", "52998224725", "joao@email.com", "11999999999");

            _clienteServiceMock
                .Setup(s => s.ObterPorId(id))
                .ReturnsAsync(response);

            IActionResult result = await _controller.ObterPorId(id);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(response, ok.Value);
        }

        [Fact]
        public async Task ObterTodos_DeveRetornarOk()
        {
            var response = new List<ClienteResponse>
            {
                new(Guid.NewGuid(), "Cliente A", "52998224725", "a@email.com", "11911111111"),
                new(Guid.NewGuid(), "Cliente B", "11144477735", "b@email.com", "11922222222")
            };

            _clienteServiceMock
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
            var request = new ClienteRequest
            {
                Nome = "Maria Silva",
                CpfCnpj = "11144477735",
                Email = "maria@email.com",
                Celular = "11888888888"
            };
            var response = new ClienteResponse(id, request.Nome, request.CpfCnpj, request.Email, request.Celular);

            _clienteServiceMock
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
            _clienteServiceMock.Verify(s => s.Deletar(id), Times.Once);
        }
    }
}
