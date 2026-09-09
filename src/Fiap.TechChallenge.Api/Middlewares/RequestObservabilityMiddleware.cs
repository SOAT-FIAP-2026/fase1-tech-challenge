using System.Diagnostics;
using Fiap.TechChallenge.Domain.Interfaces.Observability;

namespace Fiap.TechChallenge.Api.Middlewares
{
    public sealed class RequestObservabilityMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IObservabilityMetrics _metrics;
        private readonly ILogger<RequestObservabilityMiddleware> _logger;

        public RequestObservabilityMiddleware(
            RequestDelegate next,
            IObservabilityMetrics metrics,
            ILogger<RequestObservabilityMiddleware> logger)
        {
            _next = next;
            _metrics = metrics;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            long startedAt = Stopwatch.GetTimestamp();

            try
            {
                await _next(context);
            }
            finally
            {
                TimeSpan duration = Stopwatch.GetElapsedTime(startedAt);
                int statusCode = context.Response.StatusCode;
                _metrics.RecordHttpRequest(duration, context.Request.Method, statusCode);

                if (statusCode >= StatusCodes.Status500InternalServerError &&
                    context.Request.Path.StartsWithSegments("/api/v1/ordens-servico"))
                {
                    _metrics.RecordOrderProcessingFailure("http_request");
                    _logger.LogError(
                        "Falha no processamento da ordem de serviço. Método {Method}, caminho {Path}, status {StatusCode}",
                        context.Request.Method,
                        context.Request.Path,
                        statusCode);
                }
            }
        }
    }
}
