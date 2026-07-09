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
    public class OrdemServicoServiceTests
    {
        private readonly Mock<IClienteRepository> _clienteRepositoryMock = new();
        private readonly Mock<IVeiculoRepository> _veiculoRepositoryMock = new();
        private readonly Mock<IStatusOrdemServicoRepository> _statusRepositoryMock = new();
        private readonly Mock<IServicoRepository> _servicoRepositoryMock = new();
        private readonly Mock<IPecaInsumoRepository> _pecaInsumoRepositoryMock = new();
        private readonly Mock<IEstoqueRepository> _estoqueRepositoryMock = new();
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
                _estoqueRepositoryMock.Object,
                _ordemServicoRepositoryMock.Object);
        }

        [Fact]
        public async Task Criar_QuandoDadosValidos_DeveCriarOrdemComOrcamento()
        {
            Guid clienteId = Guid.NewGuid();
            Guid veiculoId = Guid.NewGuid();

            var status = new StatusOrdemServico("Recebida","RECEBIDA");
            var servico1 = new Servico("Troca de Oleo", "Troca completa", 100m);
            var servico2 = new Servico("Alinhamento", "Alinhamento dianteiro", 50m);
            var peca1 = new PecaInsumo("Filtro de Ar", 25m);

            var request = new OrdemServicoRequest
            {
                ClienteId = clienteId,
                VeiculoId = veiculoId,
                Observacao = "Teste de criacao"
            };

            _clienteRepositoryMock.Setup(r => r.ObterPorId(clienteId)).ReturnsAsync(new Cliente("Joao Silva", "52998224725", "joao@email.com", "11999999999"));
            _veiculoRepositoryMock.Setup(r => r.ObterPorId(veiculoId)).ReturnsAsync(new Veiculo("ABC1D23", "Fiat", "Argo", 2024));
            _statusRepositoryMock.Setup(r => r.ObterPorCodigo(new CodigoVO("RECEBIDA"))).ReturnsAsync(status);

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
            Assert.Empty(captured.ItensServico);
            Assert.Empty(captured.ItensPecaInsumo);
            Assert.Null(captured.Orcamento);
            _ordemServicoRepositoryMock.Verify(r => r.Adicionar(It.IsAny<OrdemServico>()), Times.Once);
        }

        [Fact]
        public async Task IniciarDiagnostico_QuandoOrdemEstaRecebida_DeveAlterarStatusParaEmDiagnostico()
        {
            var statusRecebida = new StatusOrdemServico("Recebida","RECEBIDA");
            var ordemServico = new OrdemServico(Guid.NewGuid(), Guid.NewGuid(), statusRecebida.Id, "Teste");
            var statusEmDiagnostico = new StatusOrdemServico("Em Diagnóstico","EM_DIAGNOSTICO");

            _ordemServicoRepositoryMock.Setup(r => r.ObterPorId(ordemServico.Id)).ReturnsAsync(ordemServico);
            _statusRepositoryMock.Setup(r => r.ObterPorCodigo(new CodigoVO("RECEBIDA"))).ReturnsAsync(statusRecebida);
            _statusRepositoryMock.Setup(r => r.ObterPorCodigo(new CodigoVO("EM_DIAGNOSTICO"))).ReturnsAsync(statusEmDiagnostico);

            OrdemServico? captured = null;
            _ordemServicoRepositoryMock
                .Setup(r => r.Atualizar(It.IsAny<OrdemServico>()))
                .Callback<OrdemServico>(ordem => captured = ordem)
                .Returns(Task.CompletedTask);

            OrdemServicoResponse response = await _service.IniciarDiagnostico(ordemServico.Id);

            Assert.NotNull(captured);
            Assert.Equal(statusEmDiagnostico.Id, captured!.IdStatus);
            Assert.Equal("Em Diagnóstico", response.StatusDescricao);
            _ordemServicoRepositoryMock.Verify(r => r.Atualizar(It.IsAny<OrdemServico>()), Times.Once);
        }

        [Fact]
        public async Task IniciarDiagnostico_QuandoOrdemNaoEstaRecebida_DeveLancarExcecao()
        {
            var ordemServico = new OrdemServico(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Teste");

            _ordemServicoRepositoryMock.Setup(r => r.ObterPorId(ordemServico.Id)).ReturnsAsync(ordemServico);
            _statusRepositoryMock.Setup(r => r.ObterPorCodigo(new CodigoVO("RECEBIDA"))).ReturnsAsync(new StatusOrdemServico("Recebida", "RECEBIDA"));

            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.IniciarDiagnostico(ordemServico.Id));
        }

        [Fact]
        public async Task Criar_QuandoClienteNaoExiste_DeveLancarExcecao()
        {
            var request = new OrdemServicoRequest
            {
                ClienteId = Guid.NewGuid(),
                VeiculoId = Guid.NewGuid()
            };

            _clienteRepositoryMock.Setup(r => r.ObterPorId(request.ClienteId)).ReturnsAsync((Cliente?)null);

            await Assert.ThrowsAsync<ClienteNaoEncontradoException>(() => _service.Criar(request));
        }

        [Fact]
        public async Task IncluirItens_QuandoDadosValidos_DeveAtualizarOrdemComNovoOrcamento()
        {
            var ordemServico = new OrdemServico(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Teste");
            var servicoExistente = new Servico("Troca de Oleo", "Troca completa", 100m);
            var pecaExistente = new PecaInsumo("Filtro de Ar", 25m);
            ordemServico.SincronizarItens([servicoExistente], [pecaExistente]);

            var servicoNovo = new Servico("Alinhamento", "Alinhamento dianteiro", 50m);
            var pecaNova = new PecaInsumo("Oleo Motor", 30m);
            var estoquePecaNova = new Estoque(pecaNova.Id, 10);

            _ordemServicoRepositoryMock.Setup(r => r.ObterPorId(ordemServico.Id)).ReturnsAsync(ordemServico);
            _servicoRepositoryMock.Setup(r => r.ObterPorIds(It.IsAny<IReadOnlyCollection<Guid>>())).ReturnsAsync([servicoExistente, servicoNovo]);
            _pecaInsumoRepositoryMock.Setup(r => r.ObterPorIds(It.IsAny<IReadOnlyCollection<Guid>>())).ReturnsAsync([pecaExistente, pecaNova]);
            _estoqueRepositoryMock.Setup(r => r.ObterPorIdPecaInsumo(pecaNova.Id)).ReturnsAsync(estoquePecaNova);

            OrdemServico? captured = null;
            _ordemServicoRepositoryMock
                .Setup(r => r.Atualizar(It.IsAny<OrdemServico>()))
                .Callback<OrdemServico>(ordem => captured = ordem)
                .Returns(Task.CompletedTask);

            OrdemServicoResponse response = await _service.IncluirServico(ordemServico.Id, new OrdemServicoServicosRequest
            {
                ServicosIds = [servicoNovo.Id],
            });

            OrdemServicoResponse responsePeca = await _service.IncluirPecaInsumo(ordemServico.Id, new OrdemServicoPecaInsumoRequest
            {
                PecasInsumosIds = [pecaNova.Id]
            });

            Assert.NotNull(captured);
            Assert.Equal(2, captured!.ItensServico.Count);
            Assert.Equal(2, captured.ItensPecaInsumo.Count);
            Assert.Equal(55, captured.Orcamento!.ValorTotal.Valor);
            Assert.Equal(150, response.ValorTotal);
            Assert.Equal(9, estoquePecaNova.Quantidade);
            
            _ordemServicoRepositoryMock.Verify(r => r.Atualizar(It.IsAny<OrdemServico>()), Times.Exactly(2));
        }

        [Fact]
        public async Task IncluirPecaInsumo_QuandoPecaJaExisteNaOrdem_NaoDeveBaixarEstoqueNovamente()
        {
            var ordemServico = new OrdemServico(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Teste");
            var pecaExistente = new PecaInsumo("Filtro de Ar", 25m);
            ordemServico.SincronizarItens([], [pecaExistente]);

            _ordemServicoRepositoryMock.Setup(r => r.ObterPorId(ordemServico.Id)).ReturnsAsync(ordemServico);
            _pecaInsumoRepositoryMock.Setup(r => r.ObterPorIds(It.IsAny<IReadOnlyCollection<Guid>>())).ReturnsAsync([pecaExistente]);

            await _service.IncluirPecaInsumo(ordemServico.Id, new OrdemServicoPecaInsumoRequest
            {
                PecasInsumosIds = [pecaExistente.Id]
            });

            _estoqueRepositoryMock.Verify(r => r.ObterPorIdPecaInsumo(It.IsAny<Guid>()), Times.Never);
            _ordemServicoRepositoryMock.Verify(r => r.Atualizar(ordemServico), Times.Once);
        }

        [Fact]
        public async Task RemoverItemServico_QuandoDadosValidos_DeveAtualizarOrdemComNovoOrcamento()
        {
            var ordemServico = new OrdemServico(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Teste");
            var servico1 = new Servico("Troca de Oleo", "Troca completa", 100m);
            var servico2 = new Servico("Alinhamento", "Alinhamento dianteiro", 50m);
            ordemServico.SincronizarItens([servico1, servico2], []);

            _ordemServicoRepositoryMock.Setup(r => r.ObterPorId(ordemServico.Id)).ReturnsAsync(ordemServico);
            _servicoRepositoryMock.Setup(r => r.ObterPorIds(It.IsAny<IReadOnlyCollection<Guid>>())).ReturnsAsync([servico2]);

            await _service.RemoverItemServico(ordemServico.Id, servico1.Id);

            Assert.Single(ordemServico.ItensServico);
            Assert.Equal(servico2.Id, ordemServico.ItensServico.First().IdServico);
            Assert.Equal(50m, ordemServico.Orcamento!.ValorTotal.Valor);
            _ordemServicoRepositoryMock.Verify(r => r.Atualizar(It.IsAny<OrdemServico>()), Times.Once);
        }

        [Fact]
        public async Task RemoverItemPecaInsumo_QuandoDadosValidos_DeveAtualizarOrdemComNovoOrcamento()
        {
            var ordemServico = new OrdemServico(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Teste");
            var peca1 = new PecaInsumo("Filtro de Ar", 25m);
            var peca2 = new PecaInsumo("Oleo Motor", 30m);
            ordemServico.SincronizarItens([], [peca1, peca2]);

            _ordemServicoRepositoryMock.Setup(r => r.ObterPorId(ordemServico.Id)).ReturnsAsync(ordemServico);
            _pecaInsumoRepositoryMock.Setup(r => r.ObterPorIds(It.IsAny<IReadOnlyCollection<Guid>>())).ReturnsAsync([peca2]);

            await _service.RemoverItemPecaInsumo(ordemServico.Id, peca1.Id);

            Assert.Single(ordemServico.ItensPecaInsumo);
            Assert.Equal(peca2.Id, ordemServico.ItensPecaInsumo.First().IdPecaInsumo);
            Assert.Equal(30m, ordemServico.Orcamento!.ValorTotal.Valor);
            _ordemServicoRepositoryMock.Verify(r => r.Atualizar(It.IsAny<OrdemServico>()), Times.Once);
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

        [Fact]
        public async Task RemoverItemServico_QuandoItemNaoExiste_DeveLancarExcecao()
        {
            var ordemServico = new OrdemServico(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Teste");
            _ordemServicoRepositoryMock.Setup(r => r.ObterPorId(ordemServico.Id)).ReturnsAsync(ordemServico);

            await Assert.ThrowsAsync<ItemServicoNaoEncontradoException>(() => _service.RemoverItemServico(ordemServico.Id, Guid.NewGuid()));
        }

        [Fact]
        public async Task RemoverItemPecaInsumo_QuandoItemNaoExiste_DeveLancarExcecao()
        {
            var ordemServico = new OrdemServico(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Teste");
            _ordemServicoRepositoryMock.Setup(r => r.ObterPorId(ordemServico.Id)).ReturnsAsync(ordemServico);

            await Assert.ThrowsAsync<ItemPecaInsumoNaoEncontradoException>(() => _service.RemoverItemPecaInsumo(ordemServico.Id, Guid.NewGuid()));
        }

        [Fact]
        public async Task FinalizarDiagnostico_QuandoOrdemEstaEmDiagnostico_DeveAlterarStatusParaAguardandoAprovacao()
        {
            var statusEmDiagnostico = new StatusOrdemServico("Em Diagnóstico", "EM_DIAGNOSTICO");
            var ordemServico = new OrdemServico(Guid.NewGuid(), Guid.NewGuid(), statusEmDiagnostico.Id, "Teste");
            var statusAguardandoAprovacao = new StatusOrdemServico("Aguardando aprovação", "AGUARDANDO_APROVACAO");

            _ordemServicoRepositoryMock.Setup(r => r.ObterPorId(ordemServico.Id)).ReturnsAsync(ordemServico);
            _statusRepositoryMock.Setup(r => r.ObterPorCodigo(new CodigoVO("EM_DIAGNOSTICO"))).ReturnsAsync(statusEmDiagnostico);
            _statusRepositoryMock.Setup(r => r.ObterPorCodigo(new CodigoVO("AGUARDANDO_APROVACAO"))).ReturnsAsync(statusAguardandoAprovacao);

            OrdemServico? captured = null;
            _ordemServicoRepositoryMock
                .Setup(r => r.Atualizar(It.IsAny<OrdemServico>()))
                .Callback<OrdemServico>(ordem => captured = ordem)
                .Returns(Task.CompletedTask);

            OrdemServicoResponse response = await _service.FinalizarDiagnostico(ordemServico.Id);

            Assert.NotNull(captured);
            Assert.Equal(statusAguardandoAprovacao.Id, captured!.IdStatus);
            Assert.Equal("Aguardando aprovação", response.StatusDescricao);
            _ordemServicoRepositoryMock.Verify(r => r.Atualizar(It.IsAny<OrdemServico>()), Times.Once);
        }

        [Fact]
        public async Task FinalizarDiagnostico_QuandoOrdemNaoEstaEmDiagnostico_DeveLancarExcecao()
        {
            var ordemServico = new OrdemServico(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Teste");

            _ordemServicoRepositoryMock.Setup(r => r.ObterPorId(ordemServico.Id)).ReturnsAsync(ordemServico);
            _statusRepositoryMock.Setup(r => r.ObterPorCodigo(new CodigoVO("EM_DIAGNOSTICO"))).ReturnsAsync(new StatusOrdemServico("Em Diagnóstico", "EM_DIAGNOSTICO"));

            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.FinalizarDiagnostico(ordemServico.Id));
        }

        [Fact]
        public async Task AprovarOrdemServico_QuandoOrdemEstaAguardandoAprovacao_DeveAlterarStatusParaEmExecucao()
        {
            var statusAguardandoAprovacao = new StatusOrdemServico("Aguardando aprovação", "AGUARDANDO_APROVACAO");
            var ordemServico = new OrdemServico(Guid.NewGuid(), Guid.NewGuid(), statusAguardandoAprovacao.Id, "Teste");
            var statusEmExecucao = new StatusOrdemServico("Em Execução", "EM_EXECUCAO");

            _ordemServicoRepositoryMock.Setup(r => r.ObterPorId(ordemServico.Id)).ReturnsAsync(ordemServico);
            _statusRepositoryMock.Setup(r => r.ObterPorCodigo(new CodigoVO("AGUARDANDO_APROVACAO"))).ReturnsAsync(statusAguardandoAprovacao);
            _statusRepositoryMock.Setup(r => r.ObterPorCodigo(new CodigoVO("EM_EXECUCAO"))).ReturnsAsync(statusEmExecucao);

            OrdemServico? captured = null;
            _ordemServicoRepositoryMock
                .Setup(r => r.Atualizar(It.IsAny<OrdemServico>()))
                .Callback<OrdemServico>(ordem => captured = ordem)
                .Returns(Task.CompletedTask);

            OrdemServicoResponse response = await _service.AprovarOrdemServico(ordemServico.Id);

            Assert.NotNull(captured);
            Assert.Equal(statusEmExecucao.Id, captured!.IdStatus);
            Assert.Equal("Em Execução", response.StatusDescricao);
            _ordemServicoRepositoryMock.Verify(r => r.Atualizar(It.IsAny<OrdemServico>()), Times.Once);
        }

        [Fact]
        public async Task AprovarOrdemServico_QuandoOrdemNaoEstaAguardandoAprovacao_DeveLancarExcecao()
        {
            var ordemServico = new OrdemServico(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Teste");

            _ordemServicoRepositoryMock.Setup(r => r.ObterPorId(ordemServico.Id)).ReturnsAsync(ordemServico);
            _statusRepositoryMock.Setup(r => r.ObterPorCodigo(new CodigoVO("AGUARDANDO_APROVACAO"))).ReturnsAsync(new StatusOrdemServico("Aguardando aprovação", "AGUARDANDO_APROVACAO"));

            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.AprovarOrdemServico(ordemServico.Id));
        }
    }
}
