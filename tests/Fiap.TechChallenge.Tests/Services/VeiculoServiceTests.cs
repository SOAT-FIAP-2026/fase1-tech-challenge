using Fiap.TechChallenge.Application.DTOs.Common;
using Fiap.TechChallenge.Application.DTOs.Requests;
using Fiap.TechChallenge.Application.Services;
using Fiap.TechChallenge.Domain.Entities;
using Fiap.TechChallenge.Domain.Exceptions;
using Fiap.TechChallenge.Domain.Interfaces.Repository;
using Moq;

namespace Fiap.TechChallenge.Domain.Tests.Services
{
    public class VeiculoServiceTests
    {
        private readonly Mock<IVeiculoRepository> _veiculoRepositoryMock;
        private readonly VeiculoService _veiculoService;

        public VeiculoServiceTests()
        {
            _veiculoRepositoryMock = new Mock<IVeiculoRepository>();
            _veiculoService = new VeiculoService(_veiculoRepositoryMock.Object);
        }

        #region Criar

        [Fact]
        public async Task Criar_ComDadosValidos_DeveRetornarId()
        {
            var request = new VeiculoRequest
            {
                Placa = "ABC1234",
                Marca = "Toyota",
                Modelo = "Corolla",
                Ano = 2020
            };

            _veiculoRepositoryMock
                .Setup(r => r.ExistePlaca(request.Placa, null))
                .ReturnsAsync(false);

            Guid id = await _veiculoService.Criar(request);

            Assert.NotEqual(Guid.Empty, id);
            _veiculoRepositoryMock.Verify(r => r.Adicionar(It.IsAny<Veiculo>()), Times.Once);
        }

        [Fact]
        public async Task Criar_QuandoPlacaJaExiste_DeveLancarVeiculoPlacaJaExisteException()
        {
            var request = new VeiculoRequest
            {
                Placa = "ABC1234",
                Marca = "Toyota",
                Modelo = "Corolla",
                Ano = 2020
            };

            _veiculoRepositoryMock
                .Setup(r => r.ExistePlaca(request.Placa, null))
                .ReturnsAsync(true);

            var excecao = await Assert.ThrowsAsync<VeiculoPlacaJaExisteException>(
                () => _veiculoService.Criar(request)
            );

            Assert.Contains("ABC1234", excecao.Message);
            _veiculoRepositoryMock.Verify(r => r.Adicionar(It.IsAny<Veiculo>()), Times.Never);
        }

        #endregion

        #region ObterPorId

        [Fact]
        public async Task ObterPorId_QuandoVeiculoExiste_DeveRetornarResponse()
        {
            var veiculo = new Veiculo("ABC1234", "Toyota", "Corolla", 2020);
            Guid id = veiculo.Id;

            _veiculoRepositoryMock
                .Setup(r => r.ObterPorId(id))
                .ReturnsAsync(veiculo);

            var result = await _veiculoService.ObterPorId(id);

            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
            Assert.Equal("ABC1234", result.Placa);
            Assert.Equal("Toyota", result.Marca);
            Assert.Equal("Corolla", result.Modelo);
            Assert.Equal(2020, result.Ano);
        }

        [Fact]
        public async Task ObterPorId_QuandoVeiculoNaoExiste_DeveLancarVeiculoNaoEncontradoException()
        {
            Guid id = Guid.NewGuid();

            _veiculoRepositoryMock
                .Setup(r => r.ObterPorId(id))
                .ReturnsAsync((Veiculo?)null);

            var excecao = await Assert.ThrowsAsync<VeiculoNaoEncontradoException>(
                () => _veiculoService.ObterPorId(id)
            );

            Assert.Contains(id.ToString(), excecao.Message);
        }

        #endregion

        #region ListarPaginado

        [Fact]
        public async Task ListarPaginado_DeveRetornarResultadoPaginado()
        {
            var veiculos = new List<Veiculo>
            {
                new Veiculo("ABC1234", "Toyota", "Corolla", 2020),
                new Veiculo("XYZ5678", "Honda", "Civic", 2021),
                new Veiculo("DEF1A23", "Ford", "Focus", 2019)
            };

            var request = new PagedRequest { Page = 1, PageSize = 10 };

            _veiculoRepositoryMock
                .Setup(r => r.ListarPaginado(request.Skip, request.PageSize))
                .ReturnsAsync((veiculos.AsReadOnly(), 3));

            var result = await _veiculoService.ListarPaginado(request);

            Assert.NotNull(result);
            Assert.Equal(3, result.Items.Count);
            Assert.Equal(1, result.Page);
            Assert.Equal(10, result.PageSize);
            Assert.Equal(3, result.TotalItems);
            Assert.Equal(1, result.TotalPages);
            Assert.False(result.HasPreviousPage);
            Assert.False(result.HasNextPage);
        }

        [Fact]
        public async Task ListarPaginado_ComMultiplasPaginas_DeveRetornarMetadadosCorretos()
        {
            var veiculos = new List<Veiculo>
            {
                new Veiculo("ABC1234", "Toyota", "Corolla", 2020)
            };

            var request = new PagedRequest { Page = 2, PageSize = 1 };

            _veiculoRepositoryMock
                .Setup(r => r.ListarPaginado(request.Skip, request.PageSize))
                .ReturnsAsync((veiculos.AsReadOnly(), 3));

            var result = await _veiculoService.ListarPaginado(request);

            Assert.Single(result.Items);
            Assert.Equal(2, result.Page);
            Assert.Equal(3, result.TotalPages);
            Assert.True(result.HasPreviousPage);
            Assert.True(result.HasNextPage);
        }

        [Fact]
        public async Task ListarPaginado_QuandoVazio_DeveRetornarListaVazia()
        {
            var request = new PagedRequest { Page = 1, PageSize = 10 };

            _veiculoRepositoryMock
                .Setup(r => r.ListarPaginado(request.Skip, request.PageSize))
                .ReturnsAsync((new List<Veiculo>().AsReadOnly(), 0));

            var result = await _veiculoService.ListarPaginado(request);

            Assert.NotNull(result);
            Assert.Empty(result.Items);
            Assert.Equal(0, result.TotalItems);
        }

        #endregion

        #region Atualizar (PATCH)

        [Fact]
        public async Task Atualizar_ComTodosOsCampos_DeveAtualizarERetornarResponse()
        {
            var veiculoOriginal = new Veiculo("ABC1234", "Toyota", "Corolla", 2020);
            Guid id = veiculoOriginal.Id;

            var request = new VeiculoPatchRequest
            {
                Placa = "XYZ5678",
                Marca = "Honda",
                Modelo = "Civic",
                Ano = 2022
            };

            _veiculoRepositoryMock
                .Setup(r => r.ObterPorId(id))
                .ReturnsAsync(veiculoOriginal);

            _veiculoRepositoryMock
                .Setup(r => r.ExistePlaca(request.Placa, id))
                .ReturnsAsync(false);

            var result = await _veiculoService.Atualizar(id, request);

            Assert.NotNull(result);
            Assert.Equal("XYZ5678", result.Placa);
            Assert.Equal("Honda", result.Marca);
            Assert.Equal("Civic", result.Modelo);
            Assert.Equal(2022, result.Ano);
            _veiculoRepositoryMock.Verify(r => r.Atualizar(It.IsAny<Veiculo>()), Times.Once);
        }

        [Fact]
        public async Task Atualizar_ApenasPlaca_DeveAtualizarSomenteAPlaca()
        {
            var veiculoOriginal = new Veiculo("ABC1234", "Toyota", "Corolla", 2020);
            Guid id = veiculoOriginal.Id;

            var request = new VeiculoPatchRequest
            {
                Placa = "XYZ5678"
            };

            _veiculoRepositoryMock
                .Setup(r => r.ObterPorId(id))
                .ReturnsAsync(veiculoOriginal);

            _veiculoRepositoryMock
                .Setup(r => r.ExistePlaca(request.Placa, id))
                .ReturnsAsync(false);

            var result = await _veiculoService.Atualizar(id, request);

            Assert.Equal("XYZ5678", result.Placa);
            Assert.Equal("Toyota", result.Marca);
            Assert.Equal("Corolla", result.Modelo);
            Assert.Equal(2020, result.Ano);
        }

        [Fact]
        public async Task Atualizar_ApenasMarca_DeveAtualizarSomenteAMarca()
        {
            var veiculoOriginal = new Veiculo("ABC1234", "Toyota", "Corolla", 2020);
            Guid id = veiculoOriginal.Id;

            var request = new VeiculoPatchRequest
            {
                Marca = "Honda"
            };

            _veiculoRepositoryMock
                .Setup(r => r.ObterPorId(id))
                .ReturnsAsync(veiculoOriginal);

            var result = await _veiculoService.Atualizar(id, request);

            Assert.Equal("ABC1234", result.Placa);
            Assert.Equal("Honda", result.Marca);
            Assert.Equal("Corolla", result.Modelo);
            Assert.Equal(2020, result.Ano);
        }

        [Fact]
        public async Task Atualizar_ApenasModelo_DeveAtualizarSomenteOModelo()
        {
            var veiculoOriginal = new Veiculo("ABC1234", "Toyota", "Corolla", 2020);
            Guid id = veiculoOriginal.Id;

            var request = new VeiculoPatchRequest
            {
                Modelo = "Yaris"
            };

            _veiculoRepositoryMock
                .Setup(r => r.ObterPorId(id))
                .ReturnsAsync(veiculoOriginal);

            var result = await _veiculoService.Atualizar(id, request);

            Assert.Equal("ABC1234", result.Placa);
            Assert.Equal("Toyota", result.Marca);
            Assert.Equal("Yaris", result.Modelo);
            Assert.Equal(2020, result.Ano);
        }

        [Fact]
        public async Task Atualizar_ApenasAno_DeveAtualizarSomenteOAno()
        {
            var veiculoOriginal = new Veiculo("ABC1234", "Toyota", "Corolla", 2020);
            Guid id = veiculoOriginal.Id;

            var request = new VeiculoPatchRequest
            {
                Ano = 2023
            };

            _veiculoRepositoryMock
                .Setup(r => r.ObterPorId(id))
                .ReturnsAsync(veiculoOriginal);

            var result = await _veiculoService.Atualizar(id, request);

            Assert.Equal("ABC1234", result.Placa);
            Assert.Equal("Toyota", result.Marca);
            Assert.Equal("Corolla", result.Modelo);
            Assert.Equal(2023, result.Ano);
        }

        [Fact]
        public async Task Atualizar_QuandoPlacaJaExisteEmOutroVeiculo_DeveLancarVeiculoPlacaJaExisteException()
        {
            var veiculoOriginal = new Veiculo("ABC1234", "Toyota", "Corolla", 2020);
            Guid id = veiculoOriginal.Id;

            var request = new VeiculoPatchRequest
            {
                Placa = "XYZ5678"
            };

            _veiculoRepositoryMock
                .Setup(r => r.ObterPorId(id))
                .ReturnsAsync(veiculoOriginal);

            _veiculoRepositoryMock
                .Setup(r => r.ExistePlaca(request.Placa, id))
                .ReturnsAsync(true);

            var excecao = await Assert.ThrowsAsync<VeiculoPlacaJaExisteException>(
                () => _veiculoService.Atualizar(id, request)
            );

            Assert.Contains("XYZ5678", excecao.Message);
            _veiculoRepositoryMock.Verify(r => r.Atualizar(It.IsAny<Veiculo>()), Times.Never);
        }

        [Fact]
        public async Task Atualizar_QuandoVeiculoNaoExiste_DeveLancarVeiculoNaoEncontradoException()
        {
            Guid id = Guid.NewGuid();
            var request = new VeiculoPatchRequest
            {
                Marca = "Honda"
            };

            _veiculoRepositoryMock
                .Setup(r => r.ObterPorId(id))
                .ReturnsAsync((Veiculo?)null);

            await Assert.ThrowsAsync<VeiculoNaoEncontradoException>(
                () => _veiculoService.Atualizar(id, request)
            );
        }

        [Fact]
        public async Task Atualizar_SemNenhumCampo_DeveAtualizarSemAlteracoes()
        {
            var veiculoOriginal = new Veiculo("ABC1234", "Toyota", "Corolla", 2020);
            Guid id = veiculoOriginal.Id;

            var request = new VeiculoPatchRequest();

            _veiculoRepositoryMock
                .Setup(r => r.ObterPorId(id))
                .ReturnsAsync(veiculoOriginal);

            var result = await _veiculoService.Atualizar(id, request);

            Assert.Equal("ABC1234", result.Placa);
            Assert.Equal("Toyota", result.Marca);
            Assert.Equal("Corolla", result.Modelo);
            Assert.Equal(2020, result.Ano);
            _veiculoRepositoryMock.Verify(r => r.Atualizar(It.IsAny<Veiculo>()), Times.Once);
        }

        #endregion

        #region Deletar

        [Fact]
        public async Task Deletar_QuandoVeiculoExiste_DeveChamarDeletarNoRepository()
        {
            var veiculo = new Veiculo("ABC1234", "Toyota", "Corolla", 2020);
            Guid id = veiculo.Id;

            _veiculoRepositoryMock
                .Setup(r => r.ObterPorId(id))
                .ReturnsAsync(veiculo);

            await _veiculoService.Deletar(id);

            _veiculoRepositoryMock.Verify(r => r.Deletar(It.IsAny<Veiculo>()), Times.Once);
        }

        [Fact]
        public async Task Deletar_QuandoVeiculoNaoExiste_DeveLancarVeiculoNaoEncontradoException()
        {
            Guid id = Guid.NewGuid();

            _veiculoRepositoryMock
                .Setup(r => r.ObterPorId(id))
                .ReturnsAsync((Veiculo?)null);

            await Assert.ThrowsAsync<VeiculoNaoEncontradoException>(
                () => _veiculoService.Deletar(id)
            );

            _veiculoRepositoryMock.Verify(r => r.Deletar(It.IsAny<Veiculo>()), Times.Never);
        }

        #endregion
    }
}
