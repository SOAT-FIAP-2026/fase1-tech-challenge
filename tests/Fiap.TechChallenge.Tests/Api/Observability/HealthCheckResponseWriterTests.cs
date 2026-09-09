using System.Text.Json;
using Fiap.TechChallenge.Api.Observability;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Fiap.TechChallenge.Tests.Api.Observability;

public class HealthCheckResponseWriterTests
{
    [Fact]
    public async Task WriteAsync_DeveSerializarStatusDasDependencias()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        var report = new HealthReport(
            new Dictionary<string, HealthReportEntry>
            {
                ["database"] = new(
                    HealthStatus.Unhealthy,
                    "Banco indisponível",
                    TimeSpan.FromMilliseconds(42),
                    new InvalidOperationException("connection refused"),
                    null)
            },
            TimeSpan.FromMilliseconds(50));

        await HealthCheckResponseWriter.WriteAsync(context, report);

        context.Response.ContentType.Should().Be("application/json");
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        using var document = await JsonDocument.ParseAsync(context.Response.Body);
        document.RootElement.GetProperty("status").GetString().Should().Be("unhealthy");
        document.RootElement.GetProperty("checks").GetProperty("database")
            .GetProperty("error").GetString().Should().Be("connection refused");
    }
}
