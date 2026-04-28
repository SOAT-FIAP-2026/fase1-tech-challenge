using Fiap.TechChallenge.Application.DTOs.Requests;
using Fiap.TechChallenge.Application.Services;
using Fiap.TechChallenge.Domain.Entities;
using Fiap.TechChallenge.Domain.Exceptions;
using Fiap.TechChallenge.Domain.Interfaces.Repository;
using Moq;

namespace Fiap.TechChallenge.Domain.Tests.Services
{
    public class ServicoServiceTests
    {
        private readonly Mock<IServicoRepository> _servicoRepositoryMock;
        private readonly ServicoService _servicoService;

        public ServicoServiceTests()
        {
            _servicoRepositoryMock = new Mock<IServicoRepository>();
            _servicoService = new ServicoService(_servicoRepositoryMock.Object);
        }

        [Fact]
        public async Task Criar_QuandoNomeNaoExiste_DeveRetornarId()
        {
            var request = new ServicoRequest
            {
                Nome = "Pintura",
                Descricao = "Pintura completa",
                ValorUnitario = 1500m
            };

            _servicoRepositoryMock
                .Setup(r => r.ExisteNome(request.Nome, null))
                .ReturnsAsync(false);

            Guid id = await _servicoService.Criar(request);

            Assert.NotEqual(Guid.Empty, id);
            _servicoRepositoryMock.Verify(r => r.Adicionar(It.IsAny<Servico>()), Times.Once);
        }

        [Fact]
        public async Task Criar_QuandoNomeJaExiste_DeveLancarExcecao()
        {
            var request = new ServicoRequest
            {
                Nome = "Pintura",
                Descricao = "Pintura completa",
                ValorUnitario = 1500m
            };

            _servicoRepositoryMock
                .Setup(r => r.ExisteNome(request.Nome, null))
                .ReturnsAsync(true);

            await Assert.ThrowsAsync<ServicoNomeJaExisteException>(() => _servicoService.Criar(request));
        }

        [Fact]
        public async Task ObterPorId_QuandoNaoEncontrado_DeveLancarExcecao()
        {
            Guid id = Guid.NewGuid();

            _servicoRepositoryMock
                .Setup(r => r.ObterPorId(id))
                .ReturnsAsync((Servico?)null);

            await Assert.ThrowsAsync<ServicoNaoEncontradoException>(() => _servicoService.ObterPorId(id));
        }

        [Fact]
        public async Task Atualizar_QuandoServicoExiste_DeveRetornarResponseAtualizado()
        {
            var servico = new Servico("Troca de Oleo", "Descricao antiga", 90m);
            Guid id = servico.Id;

            var request = new ServicoRequest
            {
                Nome = "Troca de Oleo Premium",
                Descricao = "Descricao nova",
                ValorUnitario = 120m
            };

            _servicoRepositoryMock
                .Setup(r => r.ObterPorId(id))
                .ReturnsAsync(servico);

            _servicoRepositoryMock
                .Setup(r => r.ExisteNome(request.Nome, id))
                .ReturnsAsync(false);

            var response = await _servicoService.Atualizar(id, request);

            Assert.Equal(id, response.Id);
            Assert.Equal(request.Nome, response.Nome);
            Assert.Equal(request.Descricao, response.Descricao);
            Assert.Equal(request.ValorUnitario, response.ValorUnitario);
            _servicoRepositoryMock.Verify(r => r.Atualizar(servico), Times.Once);
        }

        [Fact]
        public async Task Deletar_QuandoServicoExiste_DeveChamarRepositorio()
        {
            Guid id = Guid.NewGuid();
            var servico = new Servico("Polimento", "Polimento tecnico", 300m);

            _servicoRepositoryMock
                .Setup(r => r.ObterPorId(id))
                .ReturnsAsync(servico);

            await _servicoService.Deletar(id);

            _servicoRepositoryMock.Verify(r => r.Deletar(servico), Times.Once);
        }
    }
}
