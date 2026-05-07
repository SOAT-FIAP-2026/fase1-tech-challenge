using System.Net;
using System.Net.Http.Json;
using Fiap.TechChallenge.Domain.Entities;
using Fiap.TechChallenge.Infrastructure.Data;
using Fiap.TechChallenge.Tests.Api.Support;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace Fiap.TechChallenge.Tests.Api.EndToEnd
{
    public class EstoqueE2ETests(ApiWebApplicationFactory factory) : IClassFixture<ApiWebApplicationFactory>
    {
        private readonly ApiWebApplicationFactory _factory = factory;

        [Fact]
        public async Task VerificarQuantidadePorDescricao_DeveAceitarQueryStringERotaComDescricao()
        {
            HttpClient client = await _factory.CreateAuthenticatedAdminClient();
            string descricao = $"Filtro E2E {Guid.NewGuid():N}";

            await CriarEstoque(descricao, 42);

            var queryResponse = await client.GetAsync($"/api/v1/estoques/quantidade/descricao?descricao={Uri.EscapeDataString(descricao)}");
            var routeResponse = await client.GetAsync($"/api/v1/estoques/quantidade/descricao/{Uri.EscapeDataString(descricao)}");

            queryResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            routeResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            (await queryResponse.Content.ReadFromJsonAsync<int>()).Should().Be(42);
            (await routeResponse.Content.ReadFromJsonAsync<int>()).Should().Be(42);
        }

        private async Task CriarEstoque(string descricao, int quantidade)
        {
            using IServiceScope scope = _factory.Services.CreateScope();
            ApplicationDbContext context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var pecaInsumo = new PecaInsumo(descricao, 25m);
            context.PecasInsumo.Add(pecaInsumo);
            context.Estoques.Add(new Estoque(pecaInsumo.Id, quantidade));
            await context.SaveChangesAsync();
        }
    }
}
