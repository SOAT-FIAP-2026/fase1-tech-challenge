using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Fiap.TechChallenge.Application.Interfaces;
using Fiap.TechChallenge.Tests.Api.Support;
using Swashbuckle.AspNetCore.Swagger;

namespace Fiap.TechChallenge.Tests.Api
{
    public class StartupTests : IClassFixture<ApiWebApplicationFactory>
    {
        private readonly ApiWebApplicationFactory _factory;

        public StartupTests(ApiWebApplicationFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public void ConfigureServices_DeveRegistrarDependenciasEssenciais()
        {
            // Arrange & Act
            using var scope = _factory.Services.CreateScope();
            var serviceProvider = scope.ServiceProvider;

            // Assert - Verifique se os serviços registrados pelos seus métodos de extensão resolvem
            // 1. Verificando o Service de Autenticação (registrado no AddDependencyInjection)
            var authService = serviceProvider.GetService<IAutenticacaoService>();
            authService.Should().NotBeNull();

            // 2. Verificando se o Swagger foi configurado (registrado no AddSwaggerConfig)
            // O SwaggerGenerator é um serviço interno do Swagger
            var swaggerGen = serviceProvider.GetService<ISwaggerProvider>();
            swaggerGen.Should().NotBeNull();
        }

        [Theory]
        [InlineData("/swagger/v1/swagger.json")]
        public async Task Configure_DeveConfigurarMiddlewareESwaggerCorretamente(string url)
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode();
            response.Content.Headers.ContentType?.ToString().Should().Contain("application/json");
        }
    }
}
