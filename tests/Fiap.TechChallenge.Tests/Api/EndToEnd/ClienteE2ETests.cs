using System.Net;
using System.Net.Http.Json;
using Fiap.TechChallenge.Application.DTOs.Requests;
using Fiap.TechChallenge.Application.DTOs.Responses;
using Fiap.TechChallenge.Tests.Api.Support;
using FluentAssertions;

namespace Fiap.TechChallenge.Tests.Api.EndToEnd
{
    public class ClienteE2ETests(ApiWebApplicationFactory factory) : IClassFixture<ApiWebApplicationFactory>
    {
        private readonly ApiWebApplicationFactory _factory = factory;

        [Fact]
        public async Task CriarCliente_DeveExigirToken()
        {
            HttpClient client = _factory.CreateClient();

            var response = await client.PostAsJsonAsync("/api/v1/clientes", CriarClienteRequest("52998224725"));

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Cliente_DevePercorrerFluxoDeCriacaoConsultaAtualizacaoEExclusao()
        {
            HttpClient client = await _factory.CreateAuthenticatedAdminClient();
            ClienteRequest request = CriarClienteRequest("52998224725");

            var criarResponse = await client.PostAsJsonAsync("/api/v1/clientes", request);

            criarResponse.StatusCode.Should().Be(HttpStatusCode.Created);
            Guid id = await LerIdCriado(criarResponse);

            ClienteResponse? clienteCriado = await client.GetFromJsonAsync<ClienteResponse>($"/api/v1/clientes/{id}");
            clienteCriado.Should().NotBeNull();
            clienteCriado!.Nome.Should().Be(request.Nome);
            clienteCriado.CpfCnpj.Should().Be(request.CpfCnpj);

            var atualizarRequest = new ClienteRequest
            {
                Nome = "Cliente E2E Atualizado",
                CpfCnpj = "11144477735",
                Email = "cliente.e2e.atualizado@example.com",
                Celular = "11988887777"
            };

            var atualizarResponse = await client.PutAsJsonAsync($"/api/v1/clientes/{id}", atualizarRequest);
            atualizarResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            ClienteResponse? clienteAtualizado = await atualizarResponse.Content.ReadFromJsonAsync<ClienteResponse>();
            clienteAtualizado.Should().NotBeNull();
            clienteAtualizado!.Nome.Should().Be(atualizarRequest.Nome);
            clienteAtualizado.CpfCnpj.Should().Be(atualizarRequest.CpfCnpj);

            var deletarResponse = await client.DeleteAsync($"/api/v1/clientes/{id}");
            deletarResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        private static ClienteRequest CriarClienteRequest(string cpfCnpj)
        {
            return new ClienteRequest
            {
                Nome = "Cliente E2E",
                CpfCnpj = cpfCnpj,
                Email = $"cliente.{cpfCnpj}@example.com",
                Celular = "11999990000"
            };
        }

        private static async Task<Guid> LerIdCriado(HttpResponseMessage response)
        {
            IdResponse? payload = await response.Content.ReadFromJsonAsync<IdResponse>();

            return payload?.Id ?? throw new InvalidOperationException("A resposta nao retornou o id criado.");
        }

        private sealed class IdResponse
        {
            public Guid Id { get; set; }
        }
    }
}
