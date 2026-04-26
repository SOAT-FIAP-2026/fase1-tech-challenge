using Fiap.TechChallenge.Application.DTOs.Requests;
using Fiap.TechChallenge.Application.Services;
using Fiap.TechChallenge.Domain.Entities;
using Fiap.TechChallenge.Domain.Exceptions;
using Fiap.TechChallenge.Domain.Interfaces.Repository;
using Moq;

namespace Fiap.TechChallenge.Tests.Services
{
    public class EstoqueServiceTests
    {
        private readonly Mock<IEstoqueRepository> _estoqueRepositoryMock;
        private readonly Mock<IPecaInsumoRepository> _pecaInsumoRepositoryMock;
        private readonly EstoqueService _estoqueService;

        public EstoqueServiceTests()
        {
            _estoqueRepositoryMock = new Mock<IEstoqueRepository>();
            _pecaInsumoRepositoryMock = new Mock<IPecaInsumoRepository>();
            _estoqueService = new EstoqueService(_estoqueRepositoryMock.Object, _pecaInsumoRepositoryMock.Object);
        }

        [Fact]
        public async Task Criar_ComPecaValida_DeveRetornarId()
        {
            var pecaId = Guid.NewGuid();
            var peca = new PecaInsumo("Óleo de Motor", 29.99m);
            
            var request = new EstoqueRequest
            {
                IdPecaInsumo = pecaId,
                Quantidade = 100
            };

            _pecaInsumoRepositoryMock
                .Setup(r => r.ObterPorId(pecaId))
                .ReturnsAsync(peca);

            Guid id = await _estoqueService.Criar(request);

            Assert.NotEqual(Guid.Empty, id);
            _estoqueRepositoryMock.Verify(r => r.Adicionar(It.IsAny<Estoque>()), Times.Once);
        }

        [Fact]
        public async Task Criar_ComPecaInexistente_DeveLancarExcecao()
        {
            var pecaId = Guid.NewGuid();
            var request = new EstoqueRequest
            {
                IdPecaInsumo = pecaId,
                Quantidade = 100
            };

            _pecaInsumoRepositoryMock
                .Setup(r => r.ObterPorId(pecaId))
                .ReturnsAsync((PecaInsumo?)null);

            await Assert.ThrowsAsync<DomainException>(() => _estoqueService.Criar(request));
        }

        [Fact]
        public async Task VerificarQuantidadePorIdPecaInsumo_QuandoExiste_DeveRetornarQuantidade()
        {
            var pecaId = Guid.NewGuid();
            int quantidadeEsperada = 150;

            _estoqueRepositoryMock
                .Setup(r => r.VerificarQuantidadePorIdPecaInsumo(pecaId))
                .ReturnsAsync(quantidadeEsperada);

            int? resultado = await _estoqueService.VerificarQuantidadePorIdPecaInsumo(pecaId);

            Assert.NotNull(resultado);
            Assert.Equal(quantidadeEsperada, resultado.Value);
        }

        [Fact]
        public async Task VerificarQuantidadePorIdPecaInsumo_QuandoNaoExiste_DeveRetornarNull()
        {
            var pecaId = Guid.NewGuid();

            _estoqueRepositoryMock
                .Setup(r => r.VerificarQuantidadePorIdPecaInsumo(pecaId))
                .ReturnsAsync((int?)null);

            int? resultado = await _estoqueService.VerificarQuantidadePorIdPecaInsumo(pecaId);

            Assert.Null(resultado);
        }

        [Fact]
        public async Task VerificarQuantidadePorDescricaoPeca_QuandoExiste_DeveRetornarQuantidade()
        {
            string descricao = "Óleo";
            int quantidadeEsperada = 200;

            _estoqueRepositoryMock
                .Setup(r => r.VerificarQuantidadePorDescricaoPeca(descricao))
                .ReturnsAsync(quantidadeEsperada);

            int? resultado = await _estoqueService.VerificarQuantidadePorDescricaoPeca(descricao);

            Assert.NotNull(resultado);
            Assert.Equal(quantidadeEsperada, resultado.Value);
        }

        [Fact]
        public async Task VerificarQuantidadePorDescricaoPeca_QuandoNaoExiste_DeveRetornarNull()
        {
            string descricao = "Inexistente";

            _estoqueRepositoryMock
                .Setup(r => r.VerificarQuantidadePorDescricaoPeca(descricao))
                .ReturnsAsync((int?)null);

            int? resultado = await _estoqueService.VerificarQuantidadePorDescricaoPeca(descricao);

            Assert.Null(resultado);
        }

        [Fact]
        public async Task AdicionarQuantidade_ComDadosValidos_DeveAtualizarEstoque()
        {
            var pecaId = Guid.NewGuid();
            var peca = new PecaInsumo("Óleo de Motor", 29.99m);
            var estoque = new Estoque(pecaId, 100);
            int quantidadeAdicionar = 50;

            _pecaInsumoRepositoryMock
                .Setup(r => r.ObterPorId(pecaId))
                .ReturnsAsync(peca);

            _estoqueRepositoryMock
                .Setup(r => r.ObterPorIdPecaInsumo(pecaId))
                .ReturnsAsync(estoque);

            await _estoqueService.AdicionarQuantidade(pecaId, quantidadeAdicionar);

            Assert.Equal(150, estoque.Quantidade);
            _estoqueRepositoryMock.Verify(r => r.Atualizar(It.IsAny<Estoque>()), Times.Once);
        }

        [Fact]
        public async Task AdicionarQuantidade_ComPecaInexistente_DeveLancarExcecao()
        {
            var pecaId = Guid.NewGuid();

            _pecaInsumoRepositoryMock
                .Setup(r => r.ObterPorId(pecaId))
                .ReturnsAsync((PecaInsumo?)null);

            await Assert.ThrowsAsync<DomainException>(() => _estoqueService.AdicionarQuantidade(pecaId, 50));
        }

        [Fact]
        public async Task AdicionarQuantidade_ComEstoqueInexistente_DeveLancarExcecao()
        {
            var pecaId = Guid.NewGuid();
            var peca = new PecaInsumo("Óleo de Motor", 29.99m);

            _pecaInsumoRepositoryMock
                .Setup(r => r.ObterPorId(pecaId))
                .ReturnsAsync(peca);

            _estoqueRepositoryMock
                .Setup(r => r.ObterPorIdPecaInsumo(pecaId))
                .ReturnsAsync((Estoque?)null);

            await Assert.ThrowsAsync<DomainException>(() => _estoqueService.AdicionarQuantidade(pecaId, 50));
        }

        [Fact]
        public async Task RemoverQuantidade_ComDadosValidos_DeveAtualizarEstoque()
        {
            var pecaId = Guid.NewGuid();
            var peca = new PecaInsumo("Óleo de Motor", 29.99m);
            var estoque = new Estoque(pecaId, 100);
            int quantidadeRemover = 30;

            _pecaInsumoRepositoryMock
                .Setup(r => r.ObterPorId(pecaId))
                .ReturnsAsync(peca);

            _estoqueRepositoryMock
                .Setup(r => r.ObterPorIdPecaInsumo(pecaId))
                .ReturnsAsync(estoque);

            await _estoqueService.RemoverQuantidade(pecaId, quantidadeRemover);

            Assert.Equal(70, estoque.Quantidade);
            _estoqueRepositoryMock.Verify(r => r.Atualizar(It.IsAny<Estoque>()), Times.Once);
        }

        [Fact]
        public async Task RemoverQuantidade_ComQuantidadeSuperior_DeveLancarExcecao()
        {
            var pecaId = Guid.NewGuid();
            var peca = new PecaInsumo("Óleo de Motor", 29.99m);
            var estoque = new Estoque(pecaId, 100);
            int quantidadeRemover = 150;

            _pecaInsumoRepositoryMock
                .Setup(r => r.ObterPorId(pecaId))
                .ReturnsAsync(peca);

            _estoqueRepositoryMock
                .Setup(r => r.ObterPorIdPecaInsumo(pecaId))
                .ReturnsAsync(estoque);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _estoqueService.RemoverQuantidade(pecaId, quantidadeRemover));
        }

        [Fact]
        public async Task RemoverQuantidade_ComPecaInexistente_DeveLancarExcecao()
        {
            var pecaId = Guid.NewGuid();

            _pecaInsumoRepositoryMock
                .Setup(r => r.ObterPorId(pecaId))
                .ReturnsAsync((PecaInsumo?)null);

            await Assert.ThrowsAsync<DomainException>(() => _estoqueService.RemoverQuantidade(pecaId, 30));
        }

        [Fact]
        public async Task RemoverQuantidade_ComEstoqueInexistente_DeveLancarExcecao()
        {
            var pecaId = Guid.NewGuid();
            var peca = new PecaInsumo("Óleo de Motor", 29.99m);

            _pecaInsumoRepositoryMock
                .Setup(r => r.ObterPorId(pecaId))
                .ReturnsAsync(peca);

            _estoqueRepositoryMock
                .Setup(r => r.ObterPorIdPecaInsumo(pecaId))
                .ReturnsAsync((Estoque?)null);

            await Assert.ThrowsAsync<DomainException>(() => _estoqueService.RemoverQuantidade(pecaId, 30));
        }

        [Fact]
        public async Task Deletar_DeveExecutarDeletarNoRepository()
        {
            Guid estoqueId = Guid.NewGuid();

            await _estoqueService.Deletar(estoqueId);

            _estoqueRepositoryMock.Verify(r => r.Deletar(estoqueId), Times.Once);
        }
    }
}
