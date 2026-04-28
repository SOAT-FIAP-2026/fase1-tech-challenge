using Fiap.TechChallenge.Application.DTOs.Requests;
using Fiap.TechChallenge.Application.Services;
using Fiap.TechChallenge.Domain.Entities;
using Fiap.TechChallenge.Domain.Interfaces.Repository;
using Moq;

namespace Fiap.TechChallenge.Domain.Tests.Services
{
    public class PecaInsumoServiceTests
    {
        private readonly Mock<IPecaInsumoRepository> _pecaInsumoRepositoryMock;
        private readonly PecaInsumoService _pecaInsumoService;

        public PecaInsumoServiceTests()
        {
            _pecaInsumoRepositoryMock = new Mock<IPecaInsumoRepository>();
            _pecaInsumoService = new PecaInsumoService(_pecaInsumoRepositoryMock.Object);
        }

        [Fact]
        public async Task Criar_ComDadosValidos_DeveRetornarId()
        {
            var request = new PecaInsumoRequest
            {
                Descricao = "Óleo de Motor",
                ValorUnitario = 29.99m
            };

            Guid id = await _pecaInsumoService.Criar(request);

            Assert.NotEqual(Guid.Empty, id);
            _pecaInsumoRepositoryMock.Verify(r => r.Adicionar(It.IsAny<PecaInsumo>()), Times.Once);
        }

        [Fact]
        public async Task ObterPorId_QuandoEncontrado_DeveRetornarResponse()
        {
            var pecaInsumo = new PecaInsumo("Pastilha de Freio", 49.99m);
            Guid id = pecaInsumo.Id;

            _pecaInsumoRepositoryMock
                .Setup(r => r.ObterPorId(id))
                .ReturnsAsync(pecaInsumo);

            var result = await _pecaInsumoService.ObterPorId(id);

            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
            Assert.Equal("Pastilha de Freio", result.Descricao);
            Assert.Equal(49.99m, result.ValorUnitario);
        }

        [Fact]
        public async Task ObterPorId_QuandoNaoEncontrado_DeveLancarExcecao()
        {
            Guid id = Guid.NewGuid();

            _pecaInsumoRepositoryMock
                .Setup(r => r.ObterPorId(id))
                .ReturnsAsync((PecaInsumo?)null);

            await Assert.ThrowsAsync<NullReferenceException>(() => _pecaInsumoService.ObterPorId(id));
        }

        [Fact]
        public async Task ObterPorDescricao_QuandoEncontrado_DeveRetornarResponse()
        {
            var pecaInsumo = new PecaInsumo("Água Radiador", 24.99m);

            _pecaInsumoRepositoryMock
                .Setup(r => r.ObterPorDescricao("Água"))
                .ReturnsAsync(pecaInsumo);

            var result = await _pecaInsumoService.ObterPorDescricao("Água");

            Assert.NotNull(result);
            Assert.Equal("Água Radiador", result.Descricao);
        }

        [Fact]
        public async Task ObterPorDescricao_QuandoNaoEncontrado_DeveLancarExcecao()
        {
            _pecaInsumoRepositoryMock
                .Setup(r => r.ObterPorDescricao("Inexistente"))
                .ReturnsAsync((PecaInsumo?)null);

            await Assert.ThrowsAsync<NullReferenceException>(() => _pecaInsumoService.ObterPorDescricao("Inexistente"));
        }

        [Fact]
        public async Task ObterTodos_DeveRetornarListaDePecas()
        {
            var pecas = new List<PecaInsumo>
            {
                new PecaInsumo("Óleo de Motor", 29.99m),
                new PecaInsumo("Filtro de Ar", 19.99m),
                new PecaInsumo("Velas de Ignição", 79.99m)
            };

            _pecaInsumoRepositoryMock
                .Setup(r => r.ListarTodos())
                .ReturnsAsync(pecas.AsReadOnly());

            var result = await _pecaInsumoService.ObterTodos();

            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
            _pecaInsumoRepositoryMock.Verify(r => r.ListarTodos(), Times.Once);
        }

        [Fact]
        public async Task Atualizar_QuandoPecaExiste_DeveRetornarResponseAtualizado()
        {
            var pecaOriginal = new PecaInsumo("Óleo de Motor", 29.99m);
            Guid id = pecaOriginal.Id;

            var request = new PecaInsumoRequest
            {
                Descricao = "Óleo de Motor Premium",
                ValorUnitario = 35.99m
            };

            _pecaInsumoRepositoryMock
                .Setup(r => r.ObterPorId(id))
                .ReturnsAsync(pecaOriginal);

            var result = await _pecaInsumoService.Atualizar(id, request);

            Assert.NotNull(result);
            Assert.Equal("Óleo de Motor Premium", result.Descricao);
            Assert.Equal(35.99m, result.ValorUnitario);
            _pecaInsumoRepositoryMock.Verify(r => r.Atualizar(It.IsAny<PecaInsumo>()), Times.Once);
        }

        [Fact]
        public async Task Atualizar_QuandoPecaNaoExiste_DeveLancarExcecao()
        {
            Guid id = Guid.NewGuid();
            var request = new PecaInsumoRequest
            {
                Descricao = "Óleo de Motor",
                ValorUnitario = 29.99m
            };

            _pecaInsumoRepositoryMock
                .Setup(r => r.ObterPorId(id))
                .ReturnsAsync((PecaInsumo?)null);

            await Assert.ThrowsAsync<NullReferenceException>(() => _pecaInsumoService.Atualizar(id, request));
        }

        [Fact]
        public async Task Deletar_QuandoPecaExiste_DeveExecutarDeletarNoRepository()
        {
            var pecaInsumo = new PecaInsumo("Óleo de Motor", 29.99m);
            Guid id = pecaInsumo.Id;

            _pecaInsumoRepositoryMock
                .Setup(r => r.ObterPorId(id))
                .ReturnsAsync(pecaInsumo);

            await _pecaInsumoService.Deletar(id);

            _pecaInsumoRepositoryMock.Verify(r => r.Deletar(It.IsAny<PecaInsumo>()), Times.Once);
        }
    }
}
