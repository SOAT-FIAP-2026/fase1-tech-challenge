using Fiap.TechChallenge.Api.Configurations;
using Fiap.TechChallenge.Api.Observability;
using Fiap.TechChallenge.Domain.Interfaces.Observability;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace Fiap.TechChallenge.Tests.Api.Configurations;

public class ObservabilityConfigTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("http://otel-collector:4318")]
    public void AddObservability_DeveRegistrarMetricasEHealthChecks(string? otlpEndpoint)
    {
        var values = new Dictionary<string, string?>
        {
            ["OTEL_SERVICE_NAME"] = "techchallenge-api-test"
        };
        if (otlpEndpoint is not null)
            values["OTEL_EXPORTER_OTLP_ENDPOINT"] = otlpEndpoint;

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(values)
            .Build();
        var services = new ServiceCollection();

        services.AddObservability(configuration);

        services.Should().Contain(descriptor => descriptor.ServiceType == typeof(ObservabilityMetrics));
        services.Should().Contain(descriptor => descriptor.ServiceType == typeof(IObservabilityMetrics));
        services.Should().Contain(descriptor => descriptor.ServiceType.FullName == "Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckService");

        using var serviceProvider = services.BuildServiceProvider();
        serviceProvider.GetRequiredService<TracerProvider>().Should().NotBeNull();
        serviceProvider.GetRequiredService<MeterProvider>().Should().NotBeNull();
    }
}
