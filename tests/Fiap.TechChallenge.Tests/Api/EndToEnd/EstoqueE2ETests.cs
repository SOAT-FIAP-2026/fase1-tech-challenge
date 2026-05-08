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
        public async Task VerificarQuantidadePorDescricao_DeveAceitarQueryString()
        {
            HttpClient client = await _factory.CreateAuthenticatedAdminClient();
            string descricao = $"Filtro E2E {Guid.NewGuid():N}";

            await CriarEstoque(descricao, 42);

            var queryResponse = await client.GetAsync($"/api/v1/estoques/quantidade/descricao?descricao={Uri.EscapeDataString(descricao)}");

            queryResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            (await queryResponse.Content.ReadFromJsonAsync<int>()).Should().Be(42);
        }

        [Fact]
        public async Task Deletar_DeveRemoverEstoque_QuandoInformadoIdEstoque()
        {
            HttpClient client = await _factory.CreateAuthenticatedAdminClient();
            string descricao = $"Delete Estoque E2E {Guid.NewGuid():N}";
            (Guid idPecaInsumo, Guid idEstoque) = await CriarEstoque(descricao, 15);

            var deleteResponse = await client.DeleteAsync($"/api/v1/estoques/{idEstoque}");
            var quantidadeResponse = await client.GetAsync($"/api/v1/estoques/quantidade/peca/{idPecaInsumo}");

            deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
            quantidadeResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Deletar_DeveRemoverEstoque_QuandoInformadoIdPecaInsumo()
        {
            HttpClient client = await _factory.CreateAuthenticatedAdminClient();
            string descricao = $"Delete Peca E2E {Guid.NewGuid():N}";
            (Guid idPecaInsumo, _) = await CriarEstoque(descricao, 20);

            var deleteResponse = await client.DeleteAsync($"/api/v1/estoques/{idPecaInsumo}");
            var quantidadeResponse = await client.GetAsync($"/api/v1/estoques/quantidade/peca/{idPecaInsumo}");

            deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
            quantidadeResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        private async Task<(Guid IdPecaInsumo, Guid IdEstoque)> CriarEstoque(string descricao, int quantidade)
        {
            using IServiceScope scope = _factory.Services.CreateScope();
            ApplicationDbContext context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var pecaInsumo = new PecaInsumo(descricao, 25m);
            var estoque = new Estoque(pecaInsumo.Id, quantidade);

            context.PecasInsumo.Add(pecaInsumo);
            context.Estoques.Add(estoque);
            await context.SaveChangesAsync();

            return (pecaInsumo.Id, estoque.Id);
        }
    }
}
