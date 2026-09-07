using System.Diagnostics;

namespace Fiap.TechChallenge.Api.Middlewares
{
    public sealed class CorrelationIdMiddleware
    {
        public const string HeaderName = "X-Correlation-ID";

        private readonly RequestDelegate _next;
        private readonly ILogger<CorrelationIdMiddleware> _logger;

        public CorrelationIdMiddleware(RequestDelegate next, ILogger<CorrelationIdMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            string correlationId = GetCorrelationId(context);
            string traceId = Activity.Current?.TraceId.ToHexString() ?? context.TraceIdentifier;

            context.Response.Headers[HeaderName] = correlationId;

            using (_logger.BeginScope(new Dictionary<string, object>
            {
                ["correlation_id"] = correlationId,
                ["trace_id"] = traceId
            }))
            {
                await _next(context);
            }
        }

        private static string GetCorrelationId(HttpContext context)
        {
            string? requestedId = context.Request.Headers[HeaderName].FirstOrDefault();

            if (!string.IsNullOrWhiteSpace(requestedId) && requestedId.Length <= 128)
                return requestedId;

            return context.TraceIdentifier;
        }
    }
}
