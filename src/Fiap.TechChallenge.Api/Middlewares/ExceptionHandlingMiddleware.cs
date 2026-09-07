using System.Text.Json;
using System.Diagnostics;
using Fiap.TechChallenge.Api.Middlewares;
using Fiap.TechChallenge.Domain.Exceptions;

namespace Fiap.TechChallenge.Api.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public ExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlingMiddleware> logger
        )
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                string correlationId = context.Request.Headers[CorrelationIdMiddleware.HeaderName].FirstOrDefault()
                    ?? context.TraceIdentifier;
                string traceId = Activity.Current?.TraceId.ToHexString() ?? context.TraceIdentifier;

                _logger.LogError(
                    ex,
                    "Unhandled exception while processing request {Method} {Path} with status {StatusCode}",
                    context.Request.Method,
                    context.Request.Path,
                    GetStatusCode(ex));

                context.Response.Clear();
                context.Response.Headers[CorrelationIdMiddleware.HeaderName] = correlationId;
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = GetStatusCode(ex);

                var payload = new ErrorResponse(
                    context.Response.StatusCode,
                    GetMessage(ex),
                    traceId
                );

                var json = JsonSerializer.Serialize(payload, JsonOptions);
                await context.Response.WriteAsync(json);
            }
        }

        private static int GetStatusCode(Exception exception)
        {
            return exception switch
            {
                DomainException => StatusCodes.Status400BadRequest,
                ArgumentException => StatusCodes.Status400BadRequest,
                InvalidOperationException => StatusCodes.Status400BadRequest,
                UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
                _ => StatusCodes.Status500InternalServerError
            };
        }

        private static string GetMessage(Exception exception)
        {
            return exception switch
            {
                DomainException => exception.Message,
                ArgumentException => exception.Message,
                InvalidOperationException => exception.Message,
                UnauthorizedAccessException => exception.Message,
                _ => "An unexpected internal error occurred."
            };
        }

        private sealed record ErrorResponse(
            int StatusCode,
            string Message,
            string TraceId
        );
    }
}
