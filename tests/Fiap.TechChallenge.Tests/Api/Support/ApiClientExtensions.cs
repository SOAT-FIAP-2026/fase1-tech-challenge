using System.Net.Http.Headers;
using System.Net.Http.Json;
using Fiap.TechChallenge.Application.DTOs.Requests;

namespace Fiap.TechChallenge.Tests.Api.Support
{
    public static class ApiClientExtensions
    {
        public static async Task<HttpClient> CreateAuthenticatedAdminClient(this ApiWebApplicationFactory factory)
        {
            HttpClient client = factory.CreateClient();

            var loginResponse = await client.PostAsJsonAsync("/api/v1/Autenticacao/Login", new LoginRequest
            {
                Login = "admin@techchallenge.com",
                Senha = "Admin@123"
            });

            if (!loginResponse.IsSuccessStatusCode)
            {
                string error = await loginResponse.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Login do admin seed falhou com {(int)loginResponse.StatusCode}: {error}");
            }

            LoginResponse login = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>()
                ?? throw new InvalidOperationException("Nao foi possivel deserializar a resposta de login.");

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", login.Token);

            return client;
        }
    }
}
