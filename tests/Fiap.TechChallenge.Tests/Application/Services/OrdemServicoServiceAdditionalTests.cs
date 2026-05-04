using Fiap.TechChallenge.Application.DTOs.Requests;
using Fiap.TechChallenge.Application.DTOs.Responses;
using Fiap.TechChallenge.Application.Services;
using Fiap.TechChallenge.Domain.Entities;
using Fiap.TechChallenge.Domain.Exceptions;
using Fiap.TechChallenge.Domain.Interfaces.Repository;
using Fiap.TechChallenge.Domain.ValueObjects;
using Moq;

namespace Fiap.TechChallenge.Tests.Fiap.TechChallenge.Application.Services
{
    public class OrdemServicoServiceAdditionalTests
    {
        private readonly Mock<IClienteRepository> _clienteRepositoryMock = new();
        private readonly Mock<IVeiculoRepository> _veiculoRepositoryMock = new();
        private readonly Mock<IStatusOrdemServicoRepository> _statusRepositoryMock = new();
        private readonly Mock<IServicoRepository> _servicoRepositoryMock = new();
        private readonly Mock<IPecaInsumoRepository> _pecaInsumoRepositoryMock = new();
        private readonly Mock<IOrdemServicoRepository> _ordemServicoRepositoryMock = new();
        private readonly OrdemServicoService _service;

        public OrdemServicoServiceAdditionalTests()
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
        public async Task IniciarServico_QuandoItemExiste_DeveIniciarItemEAlterarStatusParaEmExecucao()
        {
            var ordemServico = new OrdemServico(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Teste");
            var servico = new Servico("Troca de Oleo", "Troca completa", 100m);
            ordemServico.SincronizarItens([servico], []);

            var statusEmExecucao = new StatusOrdemServico("Em Execucao", "EM_EXECUCAO");

            _ordemServicoRepositoryMock.Setup(r => r.ObterPorId(ordemServico.Id)).ReturnsAsync(ordemServico);
            _statusRepositoryMock.Setup(r => r.ObterPorCodigo(new CodigoVO("EM_EXECUCAO"))).ReturnsAsync(statusEmExecucao);

            await _service.IniciarServico(ordemServico.Id, servico.Id);

            Assert.Equal(statusEmExecucao.Id, ordemServico.IdStatus);
            Assert.NotNull(ordemServico.ItensServico.Single().DataHoraInicio);
            _ordemServicoRepositoryMock.Verify(r => r.Atualizar(ordemServico), Times.Once);
        }

        [Fact]
        public async Task IniciarServico_QuandoItemNaoExiste_DeveLancarExcecao()
        {
            var ordemServico = new OrdemServico(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Teste");

            _ordemServicoRepositoryMock.Setup(r => r.ObterPorId(ordemServico.Id)).ReturnsAsync(ordemServico);

            await Assert.ThrowsAsync<ItemServicoNaoEncontradoException>(() => _service.IniciarServico(ordemServico.Id, Guid.NewGuid()));
        }

        [Fact]
        public async Task FinalizarServico_QuandoTodosItensFinalizados_DeveFinalizarOrdem()
        {
            var ordemServico = new OrdemServico(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Teste");
            var servico = new Servico("Troca de Oleo", "Troca completa", 100m);
            ordemServico.SincronizarItens([servico], []);
            ordemServico.ItensServico.Single().IniciarServico();

            var statusFinalizada = new StatusOrdemServico("Finalizada", "FINALIZADA");

            _ordemServicoRepositoryMock.Setup(r => r.ObterPorId(ordemServico.Id)).ReturnsAsync(ordemServico);
            _statusRepositoryMock.Setup(r => r.ObterPorCodigo(new CodigoVO("FINALIZADA"))).ReturnsAsync(statusFinalizada);

            await _service.FinalizarServico(ordemServico.Id, servico.Id);

            Assert.Equal(statusFinalizada.Id, ordemServico.IdStatus);
            Assert.NotNull(ordemServico.DataConclusao);
            Assert.NotNull(ordemServico.ItensServico.Single().DataHoraFim);
            _ordemServicoRepositoryMock.Verify(r => r.Atualizar(ordemServico), Times.Once);
        }

        [Fact]
        public async Task FinalizarServico_QuandoAindaHaItemPendente_NaoDeveFinalizarOrdem()
        {
            Guid statusAtual = Guid.NewGuid();
            var ordemServico = new OrdemServico(Guid.NewGuid(), Guid.NewGuid(), statusAtual, "Teste");
            var servicoIniciado = new Servico("Troca de Oleo", "Troca completa", 100m);
            var servicoPendente = new Servico("Alinhamento", "Alinhamento dianteiro", 50m);
            ordemServico.SincronizarItens([servicoIniciado, servicoPendente], []);
            ordemServico.ItensServico.Single(item => item.IdServico == servicoIniciado.Id).IniciarServico();

            _ordemServicoRepositoryMock.Setup(r => r.ObterPorId(ordemServico.Id)).ReturnsAsync(ordemServico);

            await _service.FinalizarServico(ordemServico.Id, servicoIniciado.Id);

            Assert.Equal(statusAtual, ordemServico.IdStatus);
            Assert.Null(ordemServico.DataConclusao);
            _statusRepositoryMock.Verify(r => r.ObterPorCodigo(It.IsAny<CodigoVO>()), Times.Never);
            _ordemServicoRepositoryMock.Verify(r => r.Atualizar(ordemServico), Times.Once);
        }

        [Fact]
        public async Task ObterProgresso_QuandoNaoHaTempoEstimado_DeveRetornarProgressoCompletoSemPrevisao()
        {
            var ordemServico = new OrdemServico(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Teste");

            _ordemServicoRepositoryMock.Setup(r => r.ObterPorId(ordemServico.Id)).ReturnsAsync(ordemServico);

            OrdemServicoProgressoResponse response = await _service.ObterProgresso(ordemServico.Id);

            Assert.Equal(ordemServico.Id, response.IdOrdemServico);
            Assert.Equal(100, response.PercentualConcluido);
            Assert.Null(response.PrevisaoConclusao);
            Assert.Empty(response.Servicos);
        }

        [Fact]
        public async Task ObterProgresso_QuandoHaItensEmEstadosDiferentes_DeveCalcularPercentualEStatus()
        {
            var ordemServico = new OrdemServico(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Teste");
            var servicoFinalizado = new Servico("Troca de Oleo", "Troca completa", 100m, 60);
            var servicoEmExecucao = new Servico("Alinhamento", "Alinhamento dianteiro", 50m, 60);
            var servicoPendente = new Servico("Balanceamento", "Balanceamento", 40m, 60);

            ordemServico.SincronizarItens([servicoFinalizado, servicoEmExecucao, servicoPendente], []);

            ItemServico itemFinalizado = ordemServico.ItensServico.Single(item => item.IdServico == servicoFinalizado.Id);
            ItemServico itemEmExecucao = ordemServico.ItensServico.Single(item => item.IdServico == servicoEmExecucao.Id);
            ItemServico itemPendente = ordemServico.ItensServico.Single(item => item.IdServico == servicoPendente.Id);

            AssociarServico(itemFinalizado, servicoFinalizado);
            AssociarServico(itemEmExecucao, servicoEmExecucao);
            AssociarServico(itemPendente, servicoPendente);

            DateTime agora = DateTime.UtcNow;
            DefinirPeriodoExecucao(itemFinalizado, agora.AddMinutes(-90), agora.AddMinutes(-30));
            DefinirPeriodoExecucao(itemEmExecucao, agora.AddMinutes(-30), null);

            _ordemServicoRepositoryMock.Setup(r => r.ObterPorId(ordemServico.Id)).ReturnsAsync(ordemServico);

            OrdemServicoProgressoResponse response = await _service.ObterProgresso(ordemServico.Id);

            Assert.Equal(50, response.PercentualConcluido);
            Assert.Equal(ordemServico.DataAbertura.AddMinutes(180), response.PrevisaoConclusao);
            Assert.Contains(response.Servicos, servico => servico.IdServico == servicoFinalizado.Id && servico.Status == "Finalizado");
            Assert.Contains(response.Servicos, servico => servico.IdServico == servicoEmExecucao.Id && servico.Status.StartsWith("Em execu"));
            Assert.Contains(response.Servicos, servico => servico.IdServico == servicoPendente.Id && servico.Status == "Pendente");
        }

        [Fact]
        public async Task ObterPorId_QuandoOrdemNaoExiste_DeveLancarExcecao()
        {
            Guid ordemServicoId = Guid.NewGuid();
            _ordemServicoRepositoryMock.Setup(r => r.ObterPorId(ordemServicoId)).ReturnsAsync((OrdemServico?)null);

            await Assert.ThrowsAsync<OrdemServicoNaoEncontradaException>(() => _service.ObterPorId(ordemServicoId));
        }

        [Fact]
        public async Task Criar_QuandoVeiculoNaoExiste_DeveLancarExcecao()
        {
            var request = new OrdemServicoRequest
            {
                ClienteId = Guid.NewGuid(),
                VeiculoId = Guid.NewGuid(),
                ServicosIds = [],
                PecasInsumoIds = []
            };

            _clienteRepositoryMock.Setup(r => r.ObterPorId(request.ClienteId))
                .ReturnsAsync(new Cliente("Joao Silva", "52998224725", "joao@email.com", "11999999999"));
            _veiculoRepositoryMock.Setup(r => r.ObterPorId(request.VeiculoId)).ReturnsAsync((Veiculo?)null);

            await Assert.ThrowsAsync<VeiculoNaoEncontradoException>(() => _service.Criar(request));
        }

        [Fact]
        public async Task Criar_QuandoStatusInicialNaoExiste_DeveLancarExcecao()
        {
            var request = new OrdemServicoRequest
            {
                ClienteId = Guid.NewGuid(),
                VeiculoId = Guid.NewGuid(),
                ServicosIds = [],
                PecasInsumoIds = []
            };

            _clienteRepositoryMock.Setup(r => r.ObterPorId(request.ClienteId))
                .ReturnsAsync(new Cliente("Joao Silva", "52998224725", "joao@email.com", "11999999999"));
            _veiculoRepositoryMock.Setup(r => r.ObterPorId(request.VeiculoId))
                .ReturnsAsync(new Veiculo("ABC1D23", "Fiat", "Argo", 2024));
            _statusRepositoryMock.Setup(r => r.ObterPorCodigo(new CodigoVO("RECEBIDA"))).ReturnsAsync((StatusOrdemServico?)null);

            await Assert.ThrowsAsync<StatusOrdemServicoNaoEncontradoException>(() => _service.Criar(request));
        }

        [Fact]
        public async Task Criar_QuandoServicoNaoExiste_DeveLancarExcecao()
        {
            Guid servicoId = Guid.NewGuid();
            var request = new OrdemServicoRequest
            {
                ClienteId = Guid.NewGuid(),
                VeiculoId = Guid.NewGuid(),
                ServicosIds = [servicoId],
                PecasInsumoIds = []
            };

            _clienteRepositoryMock.Setup(r => r.ObterPorId(request.ClienteId))
                .ReturnsAsync(new Cliente("Joao Silva", "52998224725", "joao@email.com", "11999999999"));
            _veiculoRepositoryMock.Setup(r => r.ObterPorId(request.VeiculoId))
                .ReturnsAsync(new Veiculo("ABC1D23", "Fiat", "Argo", 2024));
            _statusRepositoryMock.Setup(r => r.ObterPorCodigo(new CodigoVO("RECEBIDA")))
                .ReturnsAsync(new StatusOrdemServico("Recebida", "RECEBIDA"));
            _servicoRepositoryMock.Setup(r => r.ObterPorIds(It.IsAny<IReadOnlyCollection<Guid>>())).ReturnsAsync([]);

            await Assert.ThrowsAsync<ServicoNaoEncontradoException>(() => _service.Criar(request));
        }

        [Fact]
        public async Task Criar_QuandoPecaNaoExiste_DeveLancarExcecao()
        {
            Guid pecaId = Guid.NewGuid();
            var request = new OrdemServicoRequest
            {
                ClienteId = Guid.NewGuid(),
                VeiculoId = Guid.NewGuid(),
                ServicosIds = [],
                PecasInsumoIds = [pecaId]
            };

            _clienteRepositoryMock.Setup(r => r.ObterPorId(request.ClienteId))
                .ReturnsAsync(new Cliente("Joao Silva", "52998224725", "joao@email.com", "11999999999"));
            _veiculoRepositoryMock.Setup(r => r.ObterPorId(request.VeiculoId))
                .ReturnsAsync(new Veiculo("ABC1D23", "Fiat", "Argo", 2024));
            _statusRepositoryMock.Setup(r => r.ObterPorCodigo(new CodigoVO("RECEBIDA")))
                .ReturnsAsync(new StatusOrdemServico("Recebida", "RECEBIDA"));
            _servicoRepositoryMock.Setup(r => r.ObterPorIds(It.IsAny<IReadOnlyCollection<Guid>>())).ReturnsAsync([]);
            _pecaInsumoRepositoryMock.Setup(r => r.ObterPorIds(It.IsAny<IReadOnlyCollection<Guid>>())).ReturnsAsync([]);

            await Assert.ThrowsAsync<PecaInsumoNaoEncontradaException>(() => _service.Criar(request));
        }

        private static void AssociarServico(ItemServico item, Servico servico)
        {
            typeof(ItemServico).GetProperty(nameof(ItemServico.Servico))!.SetValue(item, servico);
        }

        private static void DefinirPeriodoExecucao(ItemServico item, DateTime inicio, DateTime? fim)
        {
            typeof(ItemServico).GetProperty(nameof(ItemServico.DataHoraInicio))!.SetValue(item, inicio);
            typeof(ItemServico).GetProperty(nameof(ItemServico.DataHoraFim))!.SetValue(item, fim);
        }
    }
}
