using Fiap.TechChallenge.Application.DTOs.Responses;
using FluentAssertions;

namespace Fiap.TechChallenge.Tests.Application.DTOs.Responses
{
    public class EstoqueResponseTests
    {
        [Fact]
        public void Construtor_DeveAtribuirPropriedadesCorretamente()
        {
            // Arrange
            Guid id = Guid.NewGuid();
            Guid idPecaInsumo = Guid.NewGuid();
            int quantidade = 50;

            // Act
            var response = new EstoqueResponse(id, idPecaInsumo, quantidade);

            // Assert
            response.Id.Should().Be(id);
            response.IdPecaInsumo.Should().Be(idPecaInsumo);
            response.Quantidade.Should().Be(quantidade);
        }

        [Fact]
        public void Construtor_ComIdsDiferentes_NaoDeveConfundirId()
        {
            // Arrange
            Guid id = Guid.NewGuid();
            Guid idPecaInsumo = Guid.NewGuid();

            // Act
            var response = new EstoqueResponse(id, idPecaInsumo, 1);

            // Assert
            response.Id.Should().NotBe(response.IdPecaInsumo);
            response.Id.Should().Be(id);
            response.IdPecaInsumo.Should().Be(idPecaInsumo);
        }

        [Fact]
        public void Construtor_ComQuantidadeZero_DeveAtribuirZero()
        {
            // Arrange
            Guid id = Guid.NewGuid();
            Guid idPecaInsumo = Guid.NewGuid();

            // Act
            var response = new EstoqueResponse(id, idPecaInsumo, 0);

            // Assert
            response.Quantidade.Should().Be(0);
        }

        [Fact]
        public void Construtor_ComQuantidadeNegativa_DeveAtribuirValorNegativo()
        {
            // Arrange
            Guid id = Guid.NewGuid();
            Guid idPecaInsumo = Guid.NewGuid();

            // Act
            var response = new EstoqueResponse(id, idPecaInsumo, -10);

            // Assert
            response.Quantidade.Should().Be(-10);
        }

        [Fact]
        public void Construtor_ComGuidEmpty_DeveAtribuirGuidEmpty()
        {
            // Act
            var response = new EstoqueResponse(Guid.Empty, Guid.Empty, 0);

            // Assert
            response.Id.Should().Be(Guid.Empty);
            response.IdPecaInsumo.Should().Be(Guid.Empty);
        }

        [Fact]
        public void PropriedadeId_DeveSerMutavel()
        {
            // Arrange
            var response = new EstoqueResponse(Guid.NewGuid(), Guid.NewGuid(), 10);
            Guid novoId = Guid.NewGuid();

            // Act
            response.Id = novoId;

            // Assert
            response.Id.Should().Be(novoId);
        }

        [Fact]
        public void PropriedadeIdPecaInsumo_DeveSerMutavel()
        {
            // Arrange
            var response = new EstoqueResponse(Guid.NewGuid(), Guid.NewGuid(), 10);
            Guid novoIdPecaInsumo = Guid.NewGuid();

            // Act
            response.IdPecaInsumo = novoIdPecaInsumo;

            // Assert
            response.IdPecaInsumo.Should().Be(novoIdPecaInsumo);
        }

        [Fact]
        public void PropriedadeQuantidade_DeveSerMutavel()
        {
            // Arrange
            var response = new EstoqueResponse(Guid.NewGuid(), Guid.NewGuid(), 10);

            // Act
            response.Quantidade = 99;

            // Assert
            response.Quantidade.Should().Be(99);
        }

        [Fact]
        public void DoisInstances_ComMesmosValores_NaoDevemSerMesmaReferencia()
        {
            // Arrange
            Guid id = Guid.NewGuid();
            Guid idPecaInsumo = Guid.NewGuid();
            int quantidade = 42;

            // Act
            var response1 = new EstoqueResponse(id, idPecaInsumo, quantidade);
            var response2 = new EstoqueResponse(id, idPecaInsumo, quantidade);

            // Assert
            response1.Should().NotBeSameAs(response2);
            response1.Id.Should().Be(response2.Id);
            response1.IdPecaInsumo.Should().Be(response2.IdPecaInsumo);
            response1.Quantidade.Should().Be(response2.Quantidade);
        }
    }
}
