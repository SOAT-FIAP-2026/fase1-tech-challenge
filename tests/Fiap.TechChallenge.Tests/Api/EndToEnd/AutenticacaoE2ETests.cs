using System.Net;
using System.Net.Http.Json;
using Fiap.TechChallenge.Application.DTOs.Requests;
using Fiap.TechChallenge.Tests.Api.Support;
using FluentAssertions;

namespace Fiap.TechChallenge.Tests.Api.EndToEnd
{
    public class AutenticacaoE2ETests(ApiWebApplicationFactory factory) : IClassFixture<ApiWebApplicationFactory>
    {
        private readonly ApiWebApplicationFactory _factory = factory;

        [Fact]
        public async Task Cadastrar_DeveExigirToken()
        {
            HttpClient client = _factory.CreateClient();

            var response = await client.PostAsJsonAsync("/api/v1/Autenticacao/Cadastrar", new CadastrarRequest
            {
                Nome = "Usuario E2E",
                Email = "usuario.e2e@example.com",
                Login = "usuario.e2e@example.com",
                Senha = "Usuario@123",
                IdPermissao = Guid.NewGuid()
            });

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Login_DeveRetornarJwt_QuandoCredenciaisDoAdminSeedForemValidas()
        {
            HttpClient client = _factory.CreateClient();

            var response = await client.PostAsJsonAsync("/api/v1/Autenticacao/Login", new LoginRequest
            {
                Login = "admin@techchallenge.com",
                Senha = "Admin@123"
            });

            string responseBody = await response.Content.ReadAsStringAsync();
            response.StatusCode.Should().Be(HttpStatusCode.OK, responseBody);

            LoginResponse? login = await response.Content.ReadFromJsonAsync<LoginResponse>();
            login.Should().NotBeNull();
            login!.Token.Should().NotBeNullOrWhiteSpace();
            login.NomeUsuario.Should().Be("Administrador");
        }

        [Fact]
        public async Task Login_DeveRetornarUnauthorized_QuandoSenhaForInvalida()
        {
            HttpClient client = _factory.CreateClient();

            var response = await client.PostAsJsonAsync("/api/v1/Autenticacao/Login", new LoginRequest
            {
                Login = "admin@techchallenge.com",
                Senha = "senha-incorreta"
            });

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }
}
