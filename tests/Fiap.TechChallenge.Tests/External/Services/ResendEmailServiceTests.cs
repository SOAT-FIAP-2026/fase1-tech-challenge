using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Fiap.TechChallenge.Domain.Interfaces.Observability;
using Fiap.TechChallenge.External.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Xunit;

namespace Fiap.TechChallenge.Tests.External.Services
{
    public class ResendEmailServiceTests
    {
        [Fact]
        public async Task EnviarEmailAsync_QuandoChamado_DeveFazerRequisicaoPostComPayloadCorreto()
        {
            var apiKey = Environment.GetEnvironmentVariable("RESEND_API_KEY") ?? "re_test_fake_key";

            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            using var httpResponseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
            };

            handlerMock
               .Protected()
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>()
               )
               .ReturnsAsync(httpResponseMessage)
               .Verifiable();

            var httpClient = new HttpClient(handlerMock.Object);

            var configMock = new Mock<IConfiguration>();
            configMock.Setup(c => c["RESEND_API_KEY"]).Returns(apiKey);

            var service = new ResendEmailService(httpClient, configMock.Object);

            await service.EnviarEmailAsync("cliente@teste.com", "Assunto Teste", "<p>Teste</p>");

            handlerMock.Protected().Verify(
               "SendAsync",
               Times.Exactly(1),
               ItExpr.Is<HttpRequestMessage>(req =>
                  req.Method == HttpMethod.Post
                  && req.RequestUri != null
                  && req.RequestUri.ToString() == "https://api.resend.com/emails"
               ),
               ItExpr.IsAny<CancellationToken>()
            );

            Assert.Equal("Bearer", httpClient.DefaultRequestHeaders.Authorization?.Scheme);
            Assert.Equal(apiKey, httpClient.DefaultRequestHeaders.Authorization?.Parameter);
        }

        [Fact]
        public async Task EnviarEmailAsync_QuandoResendResponderSucesso_DeveRegistrarMetricaDeSucesso()
        {
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));
            var metricsMock = new Mock<IObservabilityMetrics>();
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["RESEND_API_KEY"] = "re_test_fake_key"
                })
                .Build();
            var service = new ResendEmailService(new HttpClient(handlerMock.Object), configuration, metricsMock.Object);

            bool enviado = await service.EnviarEmailAsync("cliente@teste.com", "Assunto", "<p>Teste</p>");

            Assert.True(enviado);
            metricsMock.Verify(
                metrics => metrics.RecordIntegrationRequest("resend", true),
                Times.Once);
            metricsMock.Verify(
                metrics => metrics.RecordIntegrationError(It.IsAny<string>(), It.IsAny<string>()),
                Times.Never);
        }

        [Fact]
        public async Task EnviarEmailAsync_QuandoResendFalhar_DeveRegistrarMetricasDeFalha()
        {
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(new HttpRequestException("Resend indisponível"));
            var metricsMock = new Mock<IObservabilityMetrics>();
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["RESEND_API_KEY"] = "re_test_fake_key"
                })
                .Build();
            var service = new ResendEmailService(new HttpClient(handlerMock.Object), configuration, metricsMock.Object);

            bool enviado = await service.EnviarEmailAsync("cliente@teste.com", "Assunto", "<p>Teste</p>");

            Assert.False(enviado);
            metricsMock.Verify(
                metrics => metrics.RecordIntegrationRequest("resend", false),
                Times.Once);
            metricsMock.Verify(
                metrics => metrics.RecordIntegrationError("resend", "send_email"),
                Times.Once);
        }

        [Fact]
        public async Task EnviarEmailAsync_QuandoRequisicaoForCancelada_DeveRegistrarFalha()
        {
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(new TaskCanceledException("Tempo esgotado"));
            var metricsMock = new Mock<IObservabilityMetrics>();
            var loggerMock = new Mock<ILogger<ResendEmailService>>();
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["RESEND_API_KEY"] = "re_test_fake_key"
                })
                .Build();
            var service = new ResendEmailService(
                new HttpClient(handlerMock.Object),
                configuration,
                metricsMock.Object,
                loggerMock.Object);

            bool enviado = await service.EnviarEmailAsync("cliente@teste.com", "Assunto", "<p>Teste</p>");

            Assert.False(enviado);
            metricsMock.Verify(
                metrics => metrics.RecordIntegrationRequest("resend", false),
                Times.Once);
            metricsMock.Verify(
                metrics => metrics.RecordIntegrationError("resend", "send_email"),
                Times.Once);
        }
    }
}
