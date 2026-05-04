using Fiap.TechChallenge.Application.DTOs.Responses;

namespace Fiap.TechChallenge.Tests.Application.DTOs.Responses
{
    public class OrdemServicoProgressoServicoResponseTests
    {
        [Fact]
        public void Construtor_DevePreencherPropriedades()
        {
            var idServico = Guid.NewGuid();
            var dataHoraInicio = new DateTime(2026, 5, 4, 10, 0, 0, DateTimeKind.Utc);
            var dataHoraFim = new DateTime(2026, 5, 4, 11, 15, 0, DateTimeKind.Utc);

            var response = new OrdemServicoProgressoServicoResponse(
                idServico,
                "Troca de óleo",
                "Concluído",
                90,
                75,
                dataHoraInicio,
                dataHoraFim);

            Assert.Equal(idServico, response.IdServico);
            Assert.Equal("Troca de óleo", response.Nome);
            Assert.Equal("Concluído", response.Status);
            Assert.Equal(90, response.TempoEstimadoMinutos);
            Assert.Equal(75, response.TempoExecutadoMinutos);
            Assert.Equal(dataHoraInicio, response.DataHoraInicio);
            Assert.Equal(dataHoraFim, response.DataHoraFim);
        }
    }
}