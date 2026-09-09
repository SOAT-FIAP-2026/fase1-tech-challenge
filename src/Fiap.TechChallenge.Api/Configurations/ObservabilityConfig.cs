using Fiap.TechChallenge.Api.Observability;
using Fiap.TechChallenge.Infrastructure.Data;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Fiap.TechChallenge.Api.Configurations
{
    public static class ObservabilityConfig
    {
        public static void AddObservability(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<ObservabilityMetrics>();
            services.AddSingleton<Fiap.TechChallenge.Domain.Interfaces.Observability.IObservabilityMetrics>(
                serviceProvider => serviceProvider.GetRequiredService<ObservabilityMetrics>());

            services.AddHealthChecks()
                .AddDbContextCheck<ApplicationDbContext>("database", tags: ["ready"]);

            string serviceName = configuration["OTEL_SERVICE_NAME"]
                ?? configuration["Observability:ServiceName"]
                ?? "techchallenge-api";
            string? otlpEndpoint = configuration["OTEL_EXPORTER_OTLP_ENDPOINT"];

            services.AddOpenTelemetry()
                .ConfigureResource(resource => resource
                    .AddService(
                        serviceName: serviceName,
                        serviceVersion: typeof(Program).Assembly.GetName().Version?.ToString(),
                        serviceInstanceId: Environment.MachineName)
                    .AddAttributes([
                        new KeyValuePair<string, object>("service.namespace", "techchallenge")
                    ]))
                .WithTracing(tracing =>
                {
                    tracing
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation();

                    if (!string.IsNullOrWhiteSpace(otlpEndpoint))
                        tracing.AddOtlpExporter();
                })
                .WithMetrics(metrics =>
                {
                    metrics
                        .AddMeter(ObservabilityMetrics.MeterName)
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddRuntimeInstrumentation()
                        .AddPrometheusExporter();

                    if (!string.IsNullOrWhiteSpace(otlpEndpoint))
                        metrics.AddOtlpExporter();
                });
        }
    }
}
