using Fiap.TechChallenge.Api.Middlewares;
using Fiap.TechChallenge.Contracts.Common;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;

namespace Fiap.TechChallenge.Tests.Api.Middlewares;

public class CorrelationIdMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_QuandoHeaderValido_DevePropagarCorrelationId()
    {
        var context = new DefaultHttpContext();
        context.Request.Headers[CorrelationIdMiddleware.HeaderName] = "correlation-test";
        string? ambientCorrelation = null;

        var middleware = new CorrelationIdMiddleware(
            _ =>
            {
                ambientCorrelation = CorrelationContext.Current;
                return Task.CompletedTask;
            },
            Mock.Of<ILogger<CorrelationIdMiddleware>>());

        await middleware.InvokeAsync(context);

        ambientCorrelation.Should().Be("correlation-test");
        context.Response.Headers[CorrelationIdMiddleware.HeaderName]
            .ToString().Should().Be("correlation-test");
        CorrelationContext.Current.Should().BeNull();
    }

    [Fact]
    public async Task InvokeAsync_QuandoHeaderInvalido_DeveUsarTraceIdentifier()
    {
        var context = new DefaultHttpContext { TraceIdentifier = "generated-trace-id" };
        context.Request.Headers[CorrelationIdMiddleware.HeaderName] = new string('x', 129);
        var middleware = new CorrelationIdMiddleware(
            _ => Task.CompletedTask,
            Mock.Of<ILogger<CorrelationIdMiddleware>>());

        await middleware.InvokeAsync(context);

        context.Response.Headers[CorrelationIdMiddleware.HeaderName]
            .ToString().Should().Be("generated-trace-id");
    }
}
