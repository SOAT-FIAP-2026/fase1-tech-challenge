using Fiap.TechChallenge.Application.DTOs.Requests;
using Fiap.TechChallenge.Application.DTOs.Responses;
using Fiap.TechChallenge.Application.Services;
using Fiap.TechChallenge.Domain.Entities;
using Fiap.TechChallenge.Domain.Exceptions;
using Fiap.TechChallenge.Domain.Interfaces.Repository;
using Moq;

namespace Fiap.TechChallenge.Domain.Tests.Services
{
    public class OrdemServicoServiceTests
    {
        private readonly Mock<IClienteRepository> _clienteRepositoryMock = new();
        private readonly Mock<IVeiculoRepository> _veiculoRepositoryMock = new();
        private readonly Mock<IStatusOrdemServicoRepository> _statusRepositoryMock = new();
        private readonly Mock<IServicoRepository> _servicoRepositoryMock = new();
        private readonly Mock<IPecaInsumoRepository> _pecaInsumoRepositoryMock = new();
        private readonly Mock<IOrdemServicoRepository> _ordemServicoRepositoryMock = new();
        private readonly OrdemServicoService _service;

        public OrdemServicoServiceTests()
        {
            _service = new OrdemServicoService(
                _clienteRepositoryMock.Object,
                _veiculoRepositoryMock.Object,
                _statusRepositoryMock.Object,
                _servicoRepositoryMock.Object,
                _pecaInsumoRepositoryMock.Object,
                _ordemServicoRepositoryMock.Object);
        }

        [Fact]
        public async Task Criar_QuandoDadosValidos_DeveCriarOrdemComOrcamento()
        {
            Guid clienteId = Guid.NewGuid();
            Guid veiculoId = Guid.NewGuid();

            var status = new StatusOrdemServico("Recebida");
            var servico1 = new Servico("Troca de Oleo", "Troca completa", 100m);
            var servico2 = new Servico("Alinhamento", "Alinhamento dianteiro", 50m);
            var peca1 = new PecaInsumo("Filtro de Ar", 25m);

            var request = new OrdemServicoRequest
            {
                ClienteId = clienteId,
                VeiculoId = veiculoId,
                Observacao = "Teste de criacao",
                ServicosIds = [servico1.Id, servico2.Id],
                PecasInsumoIds = [peca1.Id]
            };

            _clienteRepositoryMock.Setup(r => r.ObterPorId(clienteId)).ReturnsAsync(new Cliente("Joao Silva", "52998224725", "joao@email.com", "11999999999"));
            _veiculoRepositoryMock.Setup(r => r.ObterPorId(veiculoId)).ReturnsAsync(new Veiculo("ABC1D23", "Fiat", "Argo", 2024));
            _statusRepositoryMock.Setup(r => r.ObterPorDescricao("Recebida")).ReturnsAsync(status);
            _servicoRepositoryMock.Setup(r => r.ObterPorIds(It.IsAny<IReadOnlyCollection<Guid>>())).ReturnsAsync([servico1, servico2]);
            _pecaInsumoRepositoryMock.Setup(r => r.ObterPorIds(It.IsAny<IReadOnlyCollection<Guid>>())).ReturnsAsync([peca1]);

            OrdemServico? captured = null;
            _ordemServicoRepositoryMock
                .Setup(r => r.Adicionar(It.IsAny<OrdemServico>()))
                .Callback<OrdemServico>(ordem => captured = ordem)
                .Returns(Task.CompletedTask);

            Guid id = await _service.Criar(request);

            Assert.NotEqual(Guid.Empty, id);
            Assert.NotNull(captured);
            Assert.Equal(clienteId, captured!.IdCliente);
            Assert.Equal(veiculoId, captured.IdVeiculo);
            Assert.Equal(status.Id, captured.IdStatus);
            Assert.Collection(captured.ItensServico, _ => { }, _ => { });
            Assert.Single(captured.ItensPecaInsumo);
            Assert.NotNull(captured.Orcamento);
            Assert.Equal(175m, captured.Orcamento!.ValorTotal.Valor);
            _ordemServicoRepositoryMock.Verify(r => r.Adicionar(It.IsAny<OrdemServico>()), Times.Once);
        }

        [Fact]
        public async Task Criar_QuandoClienteNaoExiste_DeveLancarExcecao()
        {
            var request = new OrdemServicoRequest
            {
                ClienteId = Guid.NewGuid(),
                VeiculoId = Guid.NewGuid(),
                ServicosIds = [],
                PecasInsumoIds = []
            };

            _clienteRepositoryMock.Setup(r => r.ObterPorId(request.ClienteId)).ReturnsAsync((Cliente?)null);

            await Assert.ThrowsAsync<ClienteNaoEncontradoException>(() => _service.Criar(request));
        }

        [Fact]
        public async Task ObterPorId_QuandoExiste_DeveRetornarDetalhe()
        {
            var ordemServico = new OrdemServico(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Teste");
            ordemServico.AdicionarItemServico(new ItemServico(ordemServico.Id, Guid.NewGuid()));
            ordemServico.AdicionarItemPecaInsumo(new ItemPecaInsumo(ordemServico.Id, Guid.NewGuid()));
            ordemServico.DefinirOrcamento(new Orcamento(ordemServico.Id, 150m));

            _ordemServicoRepositoryMock.Setup(r => r.ObterPorId(ordemServico.Id)).ReturnsAsync(ordemServico);

            OrdemServicoResponse response = await _service.ObterPorId(ordemServico.Id);

            Assert.Equal(ordemServico.Id, response.Id);
            Assert.Equal(string.Empty, response.StatusDescricao);
            Assert.Single(response.ItensServico);
            Assert.Single(response.ItensPecaInsumo);
            Assert.Equal(150m, response.ValorTotal);
        }

        [Fact]
        public async Task ObterTodos_DeveRetornarListaDeDetalhes()
        {
            var ordemServico = new OrdemServico(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Teste");
            ordemServico.AdicionarItemServico(new ItemServico(ordemServico.Id, Guid.NewGuid()));
            ordemServico.DefinirOrcamento(new Orcamento(ordemServico.Id, 50m));

            _ordemServicoRepositoryMock.Setup(r => r.ObterTodos()).ReturnsAsync([ordemServico]);

            IReadOnlyCollection<OrdemServicoResponse> response = await _service.ObterTodos();

            Assert.Single(response);
            Assert.Equal(50m, response.First().ValorTotal);
        }
    }
}
