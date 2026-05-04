using Fiap.TechChallenge.Application.DTOs.Responses;

namespace Fiap.TechChallenge.Tests.Application.DTOs.Responses
{
    public class ServicoMetricaTempoResponseTests
    {
        [Fact]
        public void Construtor_DevePreencherPropriedades()
        {
            var idServico = Guid.NewGuid();

            var response = new ServicoMetricaTempoResponse(
                idServico,
                "Polimento",
                150,
                120,
                8);

            Assert.Equal(idServico, response.IdServico);
            Assert.Equal("Polimento", response.Nome);
            Assert.Equal(150, response.TempoEstimadoMinutos);
            Assert.Equal(120, response.TempoMedioExecucaoMinutos);
            Assert.Equal(8, response.QuantidadeExecucoes);
        }

        [Fact]
        public void Construtor_DeveAceitarTempoMedioNulo()
        {
            var response = new ServicoMetricaTempoResponse(
                Guid.NewGuid(),
                "Polimento",
                150,
                null,
                0);

            Assert.Null(response.TempoMedioExecucaoMinutos);
        }
    }
}