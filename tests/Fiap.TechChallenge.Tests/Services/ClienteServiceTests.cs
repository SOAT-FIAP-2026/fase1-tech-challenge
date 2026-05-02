using Fiap.TechChallenge.Application.DTOs.Requests;
using Fiap.TechChallenge.Application.Services;
using Fiap.TechChallenge.Domain.Entities;
using Fiap.TechChallenge.Domain.Exceptions;
using Fiap.TechChallenge.Domain.Interfaces.Repository;
using Moq;

namespace Fiap.TechChallenge.Domain.Tests.Services
{
    public class ClienteServiceTests
    {
        private readonly Mock<IClienteRepository> _clienteRepositoryMock;
        private readonly ClienteService _clienteService;

        public ClienteServiceTests()
        {
            _clienteRepositoryMock = new Mock<IClienteRepository>();
            _clienteService = new ClienteService(_clienteRepositoryMock.Object);
        }

        [Fact]
        public async Task Criar_QuandoCpfCnpjNaoExiste_DeveRetornarId()
        {
            var request = new ClienteRequest
            {
                Nome = "Joao Silva",
                CpfCnpj = "52998224725",
                Email = "joao@email.com",
                Celular = "11999999999"
            };

            _clienteRepositoryMock
                .Setup(r => r.ExisteCpfCnpj(request.CpfCnpj, null))
                .ReturnsAsync(false);

            Guid id = await _clienteService.Criar(request);

            Assert.NotEqual(Guid.Empty, id);
            _clienteRepositoryMock.Verify(r => r.Adicionar(It.IsAny<Cliente>()), Times.Once);
        }

        [Fact]
        public async Task Criar_QuandoCpfCnpjJaExiste_DeveLancarExcecao()
        {
            var request = new ClienteRequest
            {
                Nome = "Joao Silva",
                CpfCnpj = "52998224725",
                Email = "joao@email.com",
                Celular = "11999999999"
            };

            _clienteRepositoryMock
                .Setup(r => r.ExisteCpfCnpj(request.CpfCnpj, null))
                .ReturnsAsync(true);

            await Assert.ThrowsAsync<ClienteCpfCnpjJaExisteException>(() => _clienteService.Criar(request));
        }

        [Fact]
        public async Task ObterPorId_QuandoNaoEncontrado_DeveLancarExcecao()
        {
            Guid id = Guid.NewGuid();

            _clienteRepositoryMock
                .Setup(r => r.ObterPorId(id))
                .ReturnsAsync((Cliente?)null);

            await Assert.ThrowsAsync<ClienteNaoEncontradoException>(() => _clienteService.ObterPorId(id));
        }

        [Fact]
        public async Task ObterPorCpfCnpj_QuandoEncontrado_DeveRetornarResponse()
        {
            var cliente = new Cliente("Joao Silva", "52998224725", "joao@email.com", "11999999999");
            string cpfCnpj = "529.982.247-25";

            _clienteRepositoryMock
                .Setup(r => r.ObterPorCpfCnpj(cpfCnpj))
                .ReturnsAsync(cliente);

            var response = await _clienteService.ObterPorCpfCnpj(cpfCnpj);

            Assert.NotNull(response);
            Assert.Equal(cliente.Id, response!.Id);
            Assert.Equal("Joao Silva", response.Nome);
            Assert.Equal("52998224725", response.CpfCnpj);
            Assert.Equal("joao@email.com", response.Email);
            Assert.Equal("11999999999", response.Celular);
        }

        [Fact]
        public async Task ObterPorCpfCnpj_QuandoNaoEncontrado_DeveRetornarNull()
        {
            string cpfCnpj = "52998224725";

            _clienteRepositoryMock
                .Setup(r => r.ObterPorCpfCnpj(cpfCnpj))
                .ReturnsAsync((Cliente?)null);

            var response = await _clienteService.ObterPorCpfCnpj(cpfCnpj);

            Assert.Null(response);
        }

        [Fact]
        public async Task Atualizar_QuandoClienteExiste_DeveRetornarResponseAtualizado()
        {
            var cliente = new Cliente("Joao Silva", "52998224725", "joao@email.com", "11999999999");
            Guid id = cliente.Id;

            var request = new ClienteRequest
            {
                Nome = "Maria Silva",
                CpfCnpj = "11144477735",
                Email = "maria@email.com",
                Celular = "11888888888"
            };

            _clienteRepositoryMock
                .Setup(r => r.ObterPorId(id))
                .ReturnsAsync(cliente);

            _clienteRepositoryMock
                .Setup(r => r.ExisteCpfCnpj(request.CpfCnpj, id))
                .ReturnsAsync(false);

            var response = await _clienteService.Atualizar(id, request);

            Assert.Equal(id, response.Id);
            Assert.Equal(request.Nome, response.Nome);
            Assert.Equal(request.CpfCnpj, response.CpfCnpj);
            Assert.Equal(request.Email, response.Email);
            Assert.Equal(request.Celular, response.Celular);
            _clienteRepositoryMock.Verify(r => r.Atualizar(cliente), Times.Once);
        }

        [Fact]
        public async Task Deletar_QuandoClienteExiste_DeveChamarRepositorio()
        {
            Guid id = Guid.NewGuid();
            var cliente = new Cliente("Joao Silva", "52998224725", "joao@email.com", "11999999999");

            _clienteRepositoryMock
                .Setup(r => r.ObterPorId(id))
                .ReturnsAsync(cliente);

            await _clienteService.Deletar(id);

            _clienteRepositoryMock.Verify(r => r.Deletar(cliente), Times.Once);
        }
    }
}
