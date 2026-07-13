using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Fiap.TechChallenge.External.Services;
using Microsoft.Extensions.Configuration;
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
    }
}
