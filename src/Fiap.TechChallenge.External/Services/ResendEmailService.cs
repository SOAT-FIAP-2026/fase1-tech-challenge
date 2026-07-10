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

        public async Task EnviarEmailAsync(string para, string assunto, string corpoHtml)
        {
            var payload = new
            {
                from = "osstatusupdate@resend.dev",
                to = para,
                subject = assunto,
                html = corpoHtml
            };

            var response = await _httpClient.PostAsJsonAsync("https://api.resend.com/emails", payload);
            response.EnsureSuccessStatusCode();
        }
    }
}
