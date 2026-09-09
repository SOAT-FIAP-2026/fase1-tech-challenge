using Fiap.TechChallenge.Api.Middlewares;
using Fiap.TechChallenge.Domain.Interfaces.Observability;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;

namespace Fiap.TechChallenge.Tests.Api.Middlewares;

public class RequestObservabilityMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_QuandoRespostaForSucesso_DeveRegistrarLatencia()
    {
        var metrics = new Mock<IObservabilityMetrics>();
        var context = new DefaultHttpContext();
        context.Request.Method = HttpMethods.Get;
        context.Request.Path = "/health/live";
        var middleware = new RequestObservabilityMiddleware(
            _ => Task.CompletedTask,
            metrics.Object,
            Mock.Of<ILogger<RequestObservabilityMiddleware>>());

        await middleware.InvokeAsync(context);

        metrics.Verify(metric => metric.RecordHttpRequest(
            It.Is<TimeSpan>(duration => duration >= TimeSpan.Zero),
            HttpMethods.Get,
            StatusCodes.Status200OK), Times.Once);
        metrics.Verify(metric => metric.RecordOrderProcessingFailure(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task InvokeAsync_QuandoFalhaDeOrdemServico_DeveRegistrarFalha()
    {
        var metrics = new Mock<IObservabilityMetrics>();
        var context = new DefaultHttpContext();
        context.Request.Method = HttpMethods.Post;
        context.Request.Path = "/api/v1/ordens-servico";
        var middleware = new RequestObservabilityMiddleware(
            currentContext =>
            {
                currentContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                return Task.CompletedTask;
            },
            metrics.Object,
            Mock.Of<ILogger<RequestObservabilityMiddleware>>());

        await middleware.InvokeAsync(context);

        metrics.Verify(metric => metric.RecordHttpRequest(
            It.IsAny<TimeSpan>(),
            HttpMethods.Post,
            StatusCodes.Status500InternalServerError), Times.Once);
        metrics.Verify(metric => metric.RecordOrderProcessingFailure("http_request"), Times.Once);
    }

    [Fact]
    public async Task InvokeAsync_QuandoFalhaForaDaRotaDeOrdem_DeveNaoRegistrarFalhaDeOrdem()
    {
        var metrics = new Mock<IObservabilityMetrics>();
        var context = new DefaultHttpContext();
        context.Request.Path = "/health/ready";
        var middleware = new RequestObservabilityMiddleware(
            currentContext =>
            {
                currentContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                return Task.CompletedTask;
            },
            metrics.Object,
            Mock.Of<ILogger<RequestObservabilityMiddleware>>());

        await middleware.InvokeAsync(context);

        metrics.Verify(metric => metric.RecordOrderProcessingFailure(It.IsAny<string>()), Times.Never);
    }
}
