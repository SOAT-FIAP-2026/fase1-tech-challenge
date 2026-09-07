using System.Diagnostics.Metrics;
using Fiap.TechChallenge.Domain.Interfaces.Observability;

namespace Fiap.TechChallenge.Api.Observability
{
    public sealed class ObservabilityMetrics : IObservabilityMetrics
    {
        public const string MeterName = "Fiap.TechChallenge.Observability";

        private readonly Counter<long> _ordersCreated;
        private readonly Histogram<double> _orderStatusDuration;
        private readonly Counter<long> _orderProcessingFailures;
        private readonly Counter<long> _integrationRequests;
        private readonly Counter<long> _integrationErrors;
        private readonly Histogram<double> _httpRequestDuration;

        public ObservabilityMetrics()
        {
            var meter = new Meter(MeterName, "1.0.0");

            _ordersCreated = meter.CreateCounter<long>(
                "techchallenge.orders.created",
                unit: "{order}",
                description: "Ordens de serviço criadas.");

            _orderStatusDuration = meter.CreateHistogram<double>(
                "techchallenge.orders.status.duration",
                unit: "ms",
                description: "Tempo gasto em cada etapa do fluxo da ordem de serviço.");

            _orderProcessingFailures = meter.CreateCounter<long>(
                "techchallenge.orders.processing.failures",
                unit: "{failure}",
                description: "Falhas de processamento das ordens de serviço.");

            _integrationRequests = meter.CreateCounter<long>(
                "techchallenge.integrations.requests",
                unit: "{request}",
                description: "Chamadas realizadas para integrações externas.");

            _integrationErrors = meter.CreateCounter<long>(
                "techchallenge.integrations.errors",
                unit: "{error}",
                description: "Falhas em integrações externas.");

            _httpRequestDuration = meter.CreateHistogram<double>(
                "techchallenge.http.request.duration",
                unit: "ms",
                description: "Latência das requisições HTTP da API.");
        }

        public void RecordOrderCreated(string status) =>
            _ordersCreated.Add(1, new KeyValuePair<string, object?>("status", status));

        public void RecordOrderStatusDuration(string status, TimeSpan duration) =>
            _orderStatusDuration.Record(
                Math.Max(0, duration.TotalMilliseconds),
                new KeyValuePair<string, object?>("status", status));

        public void RecordOrderProcessingFailure(string operation) =>
            _orderProcessingFailures.Add(1, new KeyValuePair<string, object?>("operation", operation));

        public void RecordIntegrationRequest(string integration, bool succeeded) =>
            _integrationRequests.Add(
                1,
                new KeyValuePair<string, object?>("integration", integration),
                new KeyValuePair<string, object?>("result", succeeded ? "success" : "failure"));

        public void RecordIntegrationError(string integration, string operation) =>
            _integrationErrors.Add(
                1,
                new KeyValuePair<string, object?>("integration", integration),
                new KeyValuePair<string, object?>("operation", operation));

        public void RecordHttpRequest(TimeSpan duration, string method, int statusCode) =>
            _httpRequestDuration.Record(
                Math.Max(0, duration.TotalMilliseconds),
                new KeyValuePair<string, object?>("method", method),
                new KeyValuePair<string, object?>("status_code", statusCode));
    }
}
