using System.Diagnostics;
using Fiap.TechChallenge.Contracts.Common;

namespace Fiap.TechChallenge.Api.Middlewares
{
    public sealed class CorrelationIdMiddleware
    {
        public const string HeaderName = CorrelationContext.HeaderName;

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

            context.Items[HeaderName] = correlationId;
            context.Request.Headers[HeaderName] = correlationId;
            context.Response.Headers[HeaderName] = correlationId;
            Activity.Current?.SetTag(CorrelationContext.LogScopeKey, correlationId);

            using (CorrelationContext.Push(correlationId))
            using (_logger.BeginScope(new Dictionary<string, object>
            {
                [CorrelationContext.LogScopeKey] = correlationId,
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
                return requestedId.Trim();

            return context.TraceIdentifier;
        }
    }
}
