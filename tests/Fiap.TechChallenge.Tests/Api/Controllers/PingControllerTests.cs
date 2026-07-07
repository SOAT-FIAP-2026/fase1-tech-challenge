using Fiap.TechChallenge.Api.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace Fiap.TechChallenge.Tests.Api.Controllers
{
    public class PingControllerTests
    {
        private readonly PingController _controller = new();

        [Fact]
        public void Get_DeveRetornar200ComMensagemPong()
        {
            IActionResult result = _controller.Get();

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(200, objectResult.StatusCode);

            Assert.NotNull(objectResult.Value);
            var messageProperty = objectResult.Value!.GetType().GetProperty("message");
            Assert.NotNull(messageProperty);
            Assert.Equal("pong3", messageProperty!.GetValue(objectResult.Value));
        }
    }
}