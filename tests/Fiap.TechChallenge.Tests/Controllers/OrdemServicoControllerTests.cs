using Fiap.TechChallenge.Api.Controllers.V1;
using Fiap.TechChallenge.Application.DTOs.Requests;
using Fiap.TechChallenge.Application.DTOs.Responses;
using Fiap.TechChallenge.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Fiap.TechChallenge.Tests.Controllers
{
    public class OrdemServicoControllerTests
    {
        private readonly Mock<IOrdemServicoService> _serviceMock = new();
        private readonly OrdemServicoController _controller;

        public OrdemServicoControllerTests()
        {
            _controller = new OrdemServicoController(_serviceMock.Object);
        }

        [Fact]
        public async Task Criar_DeveRetornar201ComId()
        {
            var request = new OrdemServicoRequest
            {
                ClienteId = Guid.NewGuid(),
                VeiculoId = Guid.NewGuid(),
                ServicosIds = [],
                PecasInsumoIds = []
            };
            Guid id = Guid.NewGuid();

            _serviceMock.Setup(s => s.Criar(request)).ReturnsAsync(id);

            IActionResult result = await _controller.Criar(request);

            var created = Assert.IsType<ObjectResult>(result);
            Assert.Equal(201, created.StatusCode);
            Assert.NotNull(created.Value);
            var idProperty = created.Value!.GetType().GetProperty("id");
            Assert.NotNull(idProperty);
            Assert.Equal(id, idProperty!.GetValue(created.Value));
        }

        [Fact]
        public async Task IncluirItens_DeveRetornar200ComOrdemAtualizada()
        {
            Guid id = Guid.NewGuid();
            var request = new OrdemServicoItensRequest
            {
                ServicosIds = [Guid.NewGuid()],
                PecasInsumoIds = [Guid.NewGuid()]
            };
            var response = new OrdemServicoResponse(id, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Recebida", "Teste", DateTime.UtcNow, null, 205m, [], []);

            _serviceMock.Setup(s => s.IncluirItens(id, request)).ReturnsAsync(response);

            IActionResult result = await _controller.IncluirItens(id, request);

            var ok = Assert.IsType<OkObjectResult>(result);
            var retorno = Assert.IsType<OrdemServicoResponse>(ok.Value);
            Assert.Equal(205m, retorno.ValorTotal);
        }

        [Fact]
        public async Task ObterPorId_DeveRetornar200ComDetalhe()
        {
            Guid id = Guid.NewGuid();
            var response = new OrdemServicoResponse(id, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Recebida", "Teste", DateTime.UtcNow, null, 10m, [], []);

            _serviceMock.Setup(s => s.ObterPorId(id)).ReturnsAsync(response);

            IActionResult result = await _controller.ObterPorId(id);

            var ok = Assert.IsType<OkObjectResult>(result);
            var retorno = Assert.IsType<OrdemServicoResponse>(ok.Value);
            Assert.Equal("Recebida", retorno.StatusDescricao);
        }

        [Fact]
        public async Task ObterTodos_DeveRetornar200ComLista()
        {
            var response = new OrdemServicoResponse(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Recebida", null, DateTime.UtcNow, null, null, [], []);

            _serviceMock.Setup(s => s.ObterTodos()).ReturnsAsync([response]);

            IActionResult result = await _controller.ObterTodos();

            var ok = Assert.IsType<OkObjectResult>(result);
            var ordensServico = Assert.IsAssignableFrom<IReadOnlyCollection<OrdemServicoResponse>>(ok.Value);
            Assert.Single(ordensServico);
            Assert.Same(response, ordensServico.First());
        }
    }
}
