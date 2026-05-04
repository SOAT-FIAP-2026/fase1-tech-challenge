using Fiap.TechChallenge.Application.DTOs.Responses;

namespace Fiap.TechChallenge.Tests.Application.DTOs.Responses
{
    public class EstoqueResponseTests
    {
        [Fact]
        public void Construtor_DevePreencherPropriedades()
        {
            var id = Guid.NewGuid();
            var idPecaInsumo = Guid.NewGuid();
            const int quantidade = 12;

            var response = new EstoqueResponse(id, idPecaInsumo, quantidade);

            Assert.Equal(id, response.Id);
            Assert.Equal(idPecaInsumo, response.IdPecaInsumo);
            Assert.Equal(quantidade, response.Quantidade);
        }
    }
}