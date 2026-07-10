using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Fiap.TechChallenge.External.Services;
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
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            
            handlerMock
               .Protected()
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>()
               )
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.OK,
               })
               .Verifiable();

            var httpClient = new HttpClient(handlerMock.Object);
            var service = new ResendEmailService(httpClient);

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
            Assert.Equal("re_HZC6QiGu_81MT8QTRGv9PdNPXRQKTCF9V", httpClient.DefaultRequestHeaders.Authorization?.Parameter);
        }
    }
}
