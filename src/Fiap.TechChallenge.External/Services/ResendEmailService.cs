using System.Net.Http.Headers;
using System.Net.Http.Json;
using Fiap.TechChallenge.Domain.Interfaces.Observability;
using Fiap.TechChallenge.Domain.Interfaces.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Fiap.TechChallenge.External.Services
{
    public class ResendEmailService : IEmailService
    {
        private readonly HttpClient _httpClient;
        private readonly IObservabilityMetrics? _observabilityMetrics;
        private readonly ILogger<ResendEmailService>? _logger;

        public ResendEmailService(
            HttpClient httpClient,
            IConfiguration configuration,
            IObservabilityMetrics? observabilityMetrics = null,
            ILogger<ResendEmailService>? logger = null)
        {
            _httpClient = httpClient;
            _observabilityMetrics = observabilityMetrics;
            _logger = logger;
            var apiKey = configuration["RESEND_API_KEY"];
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        }

        public async Task<bool> EnviarEmailAsync(string para, string assunto, string corpoHtml)
        {
            // MOCK: O Resend só permite enviar para o e-mail cadastrado no plano gratuito
            string emailDestino = "dio_kenedy@hotmail.com";

            var payload = new
            {
                from = "osstatusupdate@resend.dev",
                to = emailDestino,
                subject = assunto,
                html = corpoHtml
            };

            int maxRetries = 3;
            for (int i = 0; i < maxRetries; i++)
            {
                try
                {
                    var response = await _httpClient.PostAsJsonAsync("https://api.resend.com/emails", payload);
                    response.EnsureSuccessStatusCode();
                    _observabilityMetrics?.RecordIntegrationRequest("resend", succeeded: true);
                    return true;
                }
                catch (HttpRequestException exception)
                {
                    if (i == maxRetries - 1)
                    {
                        RecordFailure(exception);
                        return false;
                    }

                    await Task.Delay(1000);
                }
                catch (TaskCanceledException exception)
                {
                    if (i == maxRetries - 1)
                    {
                        RecordFailure(exception);
                        return false;
                    }

                    await Task.Delay(1000);
                }
            }

            return false;
        }

        private void RecordFailure(Exception exception)
        {
            _observabilityMetrics?.RecordIntegrationRequest("resend", succeeded: false);
            _observabilityMetrics?.RecordIntegrationError("resend", "send_email");
            _logger?.LogError(exception, "Falha na integração com o Resend ao enviar e-mail");
        }
    }
}
