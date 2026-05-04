using Fiap.TechChallenge.Application.DTOs.Responses;

namespace Fiap.TechChallenge.Tests.Application.DTOs.Responses
{
    public class OrdemServicoProgressoResponseTests
    {
        [Fact]
        public void Construtor_DevePreencherPropriedades()
        {
            var servicos = new[]
            {
                new OrdemServicoProgressoServicoResponse(
                    Guid.NewGuid(),
                    "Serviço A",
                    "Em andamento",
                    120,
                    80,
                    new DateTime(2026, 5, 4, 10, 0, 0, DateTimeKind.Utc),
                    null)
            };

            var idOrdemServico = Guid.NewGuid();
            var dataAbertura = new DateTime(2026, 5, 4, 9, 0, 0, DateTimeKind.Utc);
            var previsaoConclusao = new DateTime(2026, 5, 4, 12, 0, 0, DateTimeKind.Utc);

            var response = new OrdemServicoProgressoResponse(
                idOrdemServico,
                "Em andamento",
                65,
                dataAbertura,
                previsaoConclusao,
                servicos);

            Assert.Equal(idOrdemServico, response.IdOrdemServico);
            Assert.Equal("Em andamento", response.Status);
            Assert.Equal(65, response.PercentualConcluido);
            Assert.Equal(dataAbertura, response.DataAbertura);
            Assert.Equal(previsaoConclusao, response.PrevisaoConclusao);
            Assert.Equal(servicos, response.Servicos);
        }
    }
}