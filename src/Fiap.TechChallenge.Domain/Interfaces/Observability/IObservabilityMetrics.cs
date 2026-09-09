namespace Fiap.TechChallenge.Domain.Interfaces.Observability
{
    public interface IObservabilityMetrics
    {
        void RecordOrderCreated(string status);

        void RecordOrderStatusDuration(string status, TimeSpan duration);

        void RecordOrderProcessingFailure(string operation);

        void RecordIntegrationRequest(string integration, bool succeeded);

        void RecordIntegrationError(string integration, string operation);

        void RecordHttpRequest(TimeSpan duration, string method, int statusCode);
    }
}
