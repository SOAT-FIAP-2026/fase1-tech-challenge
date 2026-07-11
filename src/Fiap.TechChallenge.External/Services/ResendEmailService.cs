using System.Net.Http.Headers;
using System.Net.Http.Json;
using Fiap.TechChallenge.Domain.Interfaces.Service;

namespace Fiap.TechChallenge.External.Services
{
    public class ResendEmailService : IEmailService
    {
        private readonly HttpClient _httpClient;
        private const string ApiKey = "re_HZC6QiGu_81MT8QTRGv9PdNPXRQKTCF9V";

        public ResendEmailService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ApiKey);
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
                    return true;
                }
                catch (Exception)
                {
                    if (i == maxRetries - 1)
                        return false;

                    await Task.Delay(1000);
                }
            }

            return false;
        }
    }
}
