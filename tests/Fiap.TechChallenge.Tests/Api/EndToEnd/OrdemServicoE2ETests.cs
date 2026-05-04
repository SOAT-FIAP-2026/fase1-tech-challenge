using System.Net;
using System.Net.Http.Json;
using Fiap.TechChallenge.Application.DTOs.Requests;
using Fiap.TechChallenge.Application.DTOs.Responses;
using Fiap.TechChallenge.Infrastructure.Data.Seed;
using Fiap.TechChallenge.Tests.Api.Support;
using FluentAssertions;

namespace Fiap.TechChallenge.Tests.Api.EndToEnd
{
    public class OrdemServicoE2ETests(ApiWebApplicationFactory factory) : IClassFixture<ApiWebApplicationFactory>
    {
        private readonly ApiWebApplicationFactory _factory = factory;

        [Fact]
        public async Task OrdemServico_DeveCriarComItensEAtualizarFluxoDeDiagnostico()
        {
            HttpClient client = await _factory.CreateAuthenticatedAdminClient();
            ClienteResponse cliente = await ObterClienteSeed(client);
            VeiculoResponse veiculo = await CriarVeiculo(client);
            ServicoResponse servico = (await client.GetFromJsonAsync<List<ServicoResponse>>("/api/v1/servicos"))!.First();
            PecaInsumoResponse peca = (await client.GetFromJsonAsync<List<PecaInsumoResponse>>("/api/v1/pecas-insumos"))!.First();

            var criarResponse = await client.PostAsJsonAsync("/api/v1/ordens-servico", new OrdemServicoRequest
            {
                ClienteId = cliente.Id,
                VeiculoId = veiculo.Id,
                Observacao = "Fluxo E2E",
                ServicosIds = [servico.Id],
                PecasInsumoIds = [peca.Id]
            });

            criarResponse.StatusCode.Should().Be(HttpStatusCode.Created);
            Guid ordemId = await LerIdCriado(criarResponse);

            OrdemServicoResponse ordemCriada = (await client.GetFromJsonAsync<OrdemServicoResponse>($"/api/v1/ordens-servico/{ordemId}"))!;
            ordemCriada.IdStatus.Should().Be(DatabaseSeed.StatusRecebidaId);
            ordemCriada.ItensServico.Should().ContainSingle(item => item.IdServico == servico.Id);
            ordemCriada.ItensPecaInsumo.Should().ContainSingle(item => item.IdPecaInsumo == peca.Id);
            ordemCriada.ValorTotal.Should().Be(servico.ValorUnitario + peca.ValorUnitario);

            var iniciarDiagnosticoResponse = await client.PatchAsync($"/api/v1/ordens-servico/{ordemId}/iniciar-diagnostico", null);
            iniciarDiagnosticoResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            OrdemServicoResponse ordemEmDiagnostico = (await iniciarDiagnosticoResponse.Content.ReadFromJsonAsync<OrdemServicoResponse>())!;
            ordemEmDiagnostico.IdStatus.Should().Be(DatabaseSeed.StatusEmDiagnosticoId);

            var finalizarDiagnosticoResponse = await client.PatchAsync($"/api/v1/ordens-servico/{ordemId}/finalizar-diagnostico", null);
            finalizarDiagnosticoResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            OrdemServicoResponse ordemAguardandoAprovacao = (await finalizarDiagnosticoResponse.Content.ReadFromJsonAsync<OrdemServicoResponse>())!;
            ordemAguardandoAprovacao.IdStatus.Should().Be(DatabaseSeed.StatusEmAguardandoAprovacaoId);
        }

        private static async Task<ClienteResponse> ObterClienteSeed(HttpClient client)
        {
            List<ClienteResponse> clientes = (await client.GetFromJsonAsync<List<ClienteResponse>>("/api/v1/clientes"))!;

            return clientes.First();
        }

        private static async Task<VeiculoResponse> CriarVeiculo(HttpClient client)
        {
            var response = await client.PostAsJsonAsync("/api/v1/veiculos", new VeiculoRequest
            {
                Placa = "TST1A23",
                Marca = "Honda",
                Modelo = "Civic",
                Ano = 2020
            });

            response.StatusCode.Should().Be(HttpStatusCode.Created);
            Guid id = await LerIdCriado(response);

            return (await client.GetFromJsonAsync<VeiculoResponse>($"/api/v1/veiculos/{id}"))!;
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
