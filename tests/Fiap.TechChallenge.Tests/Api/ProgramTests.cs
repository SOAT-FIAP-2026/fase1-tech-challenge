using Fiap.TechChallenge.Infrastructure.Data;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using Xunit;

namespace Fiap.TechChallenge.Tests.Api
{
    public class ProgramTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public ProgramTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Application_DeveSubirEResponderComSucesso()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act - Tenta acessar o Swagger para validar que o pipeline está OK
            var response = await client.GetAsync("/swagger/v1/swagger.json");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Content.Headers.ContentType?.ToString().Should().Contain("application/json");
        }

        [Fact]
        public void ServiceProvider_DeveResolverDependenciasSemErros()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var services = scope.ServiceProvider;

            // Act & Assert - Verifica se o contexto do banco foi registrado corretamente
            var dbContext = services.GetService<ApplicationDbContext>();
            dbContext.Should().NotBeNull();
        }
    }
}