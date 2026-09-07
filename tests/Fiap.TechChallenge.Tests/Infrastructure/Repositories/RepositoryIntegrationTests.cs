using Fiap.TechChallenge.Domain.Entities;
using Fiap.TechChallenge.Infrastructure.Data;
using Fiap.TechChallenge.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;

namespace Fiap.TechChallenge.Tests.Infrastructure.Repositories
{
    public class RepositoryIntegrationTests
    {
        [Fact]
        public async Task EstoqueRepository_DeveCriarConsultarAtualizarEDeletarEstoque()
        {
            await using var database = await CreateDatabase();
            var peca = new PecaInsumo("Sensor de Estacionamento", 129.90m);
            database.Context.PecasInsumo.Add(peca);
            await database.Context.SaveChangesAsync();

            var repository = new EstoqueRepository(database.Context);
            var estoque = new Estoque(peca.Id, 10);

            await repository.Adicionar(estoque);

            (await repository.VerificarQuantidadePorIdPecaInsumo(peca.Id)).Should().Be(10);
            (await repository.ObterPorId(estoque.Id)).Should().BeEquivalentTo(estoque);
            (await repository.ObterPorIdPecaInsumo(peca.Id)).Should().BeEquivalentTo(estoque);
            (await repository.VerificarQuantidadePorDescricaoPeca("Sensor")).Should().Be(10);

            estoque.AdicionarQuantidade(5);
            await repository.Atualizar(estoque);

            (await repository.VerificarQuantidadePorIdPecaInsumo(peca.Id)).Should().Be(15);

            await repository.Deletar(estoque.Id);
            await repository.Deletar(Guid.NewGuid());

            (await repository.VerificarQuantidadePorIdPecaInsumo(peca.Id)).Should().BeNull();
        }

        [Fact]
        public async Task ServicoRepository_DeveCriarListarAtualizarEDeletarServico()
        {
            await using var database = await CreateDatabase();
            var repository = new ServicoRepository(database.Context);
            var alinhamento = new Servico("Alinhamento Premium", "Alinhamento completo", 180m, 50);
            var balanceamento = new Servico("Balanceamento Premium", "Balanceamento completo", 120m, 40);

            await repository.Adicionar(balanceamento);
            await repository.Adicionar(alinhamento);

            (await repository.ObterPorId(alinhamento.Id)).Should().NotBeNull();
            (await repository.ObterPorIds([alinhamento.Id, alinhamento.Id, Guid.NewGuid()]))
                .Should()
                .ContainSingle(s => s.Id == alinhamento.Id);

            IReadOnlyCollection<Servico> todos = await repository.ObterTodos();
            todos.Select(s => s.Nome.Valor).Should().Equal("Alinhamento Premium", "Balanceamento Premium");

            (await repository.ExisteNome("Alinhamento Premium")).Should().BeTrue();
            (await repository.ExisteNome("Alinhamento Premium", alinhamento.Id)).Should().BeFalse();
            (await repository.ExisteNome("Servico Inexistente")).Should().BeFalse();

            alinhamento.Atualizar("Alinhamento 3D", "Alinhamento atualizado", 220m, 60);
            await repository.Atualizar(alinhamento);

            (await repository.ObterPorId(alinhamento.Id))!.Nome.Valor.Should().Be("Alinhamento 3D");

            await repository.Deletar(alinhamento);

            (await repository.ObterPorId(alinhamento.Id)).Should().BeNull();
        }

        [Fact]
        public async Task VeiculoRepository_DeveCriarListarAtualizarEDeletarVeiculo()
        {
            await using var database = await CreateDatabase();
            var repository = new VeiculoRepository(database.Context);
            var civic = new Veiculo("CAR1A23", "Honda", "Civic", 2020);
            var corolla = new Veiculo("CAR1B23", "Toyota", "Corolla", 2021);

            await repository.Adicionar(corolla);
            await repository.Adicionar(civic);

            (await repository.ObterPorId(civic.Id)).Should().NotBeNull();
            (await repository.ObterPorPlaca("car-1a23"))!.Id.Should().Be(civic.Id);
            (await repository.ExistePlaca("CAR1A23")).Should().BeTrue();
            (await repository.ExistePlaca("CAR1A23", civic.Id)).Should().BeFalse();
            (await repository.ExistePlaca("ZZZ9Z99")).Should().BeFalse();

            var (items, totalCount) = await repository.ListarPaginado(skip: 0, take: 10);
            totalCount.Should().Be(2);
            items.Select(v => v.Marca.Valor).Should().Equal("Honda", "Toyota");

            civic.AlterarModelo("Civic Touring");
            await repository.Atualizar(civic);

            (await repository.ObterPorId(civic.Id))!.Modelo.Valor.Should().Be("Civic Touring");

            await repository.Deletar(civic);

            (await repository.ObterPorId(civic.Id)).Should().BeNull();
        }

        [Fact]
        public async Task PecaInsumoRepository_DeveCriarListarAtualizarEDeletarPeca()
        {
            await using var database = await CreateDatabase();
            var repository = new PecaInsumoRepository(database.Context);
            var filtro = new PecaInsumo("Filtro de Cabine", 55m);
            var lampada = new PecaInsumo("Lampada H7", 42m);

            await repository.Adicionar(filtro);
            await repository.Adicionar(lampada);

            (await repository.ObterPorId(filtro.Id)).Should().NotBeNull();
            (await repository.ObterPorDescricao("Filtro"))!.Id.Should().Be(filtro.Id);
            (await repository.ListarTodos()).Should().HaveCount(2);
            (await repository.ObterPorIds([filtro.Id, filtro.Id, Guid.NewGuid()]))
                .Should()
                .ContainSingle(p => p.Id == filtro.Id);

            filtro.AlterarDescricao("Filtro de Cabine Premium");
            filtro.AlterarValorUnitario(65m);
            await repository.Atualizar(filtro);

            PecaInsumo? pecaAtualizada = await repository.ObterPorId(filtro.Id);
            pecaAtualizada!.Descricao.Valor.Should().Be("Filtro de Cabine Premium");
            pecaAtualizada.ValorUnitario.Valor.Should().Be(65m);

            await repository.Deletar(filtro);

            (await repository.ObterPorId(filtro.Id)).Should().BeNull();
        }

        [Fact]
        public async Task OrdemServicoRepository_ObterTodos_DeveOrdenarPorPrioridadeEAntiguidadeEExcluirConcluidas()
        {
            await using var database = await CreateDatabase();
            var cliente = new Cliente("Cliente da Oficina", "52998224725", "cliente@oficina.com", "11999999999");
            var veiculo = new Veiculo("ORD1A23", "Honda", "Civic", 2020);
            var recebida = new StatusOrdemServico("Recebida", StatusOS.Recebida.Codigo);
            var diagnostico = new StatusOrdemServico("Em Diagnostico", StatusOS.EmDiagnostico.Codigo);
            var aguardandoAprovacao = new StatusOrdemServico("Aguardando Aprovacao", StatusOS.AguardandoAprovacao.Codigo);
            var emExecucao = new StatusOrdemServico("Em Execucao", StatusOS.EmExecucao.Codigo);
            var finalizada = new StatusOrdemServico("Finalizada", StatusOS.Finalizada.Codigo);
            var entregue = new StatusOrdemServico("Entregue", StatusOS.Entregue.Codigo);
            var cancelada = new StatusOrdemServico("Cancelada", StatusOS.Cancelada.Codigo);

            database.Context.AddRange(
                cliente,
                veiculo,
                recebida,
                diagnostico,
                aguardandoAprovacao,
                emExecucao,
                finalizada,
                entregue,
                cancelada);
            await database.Context.SaveChangesAsync();

            DateTime agora = DateTime.UtcNow;
            var execucaoAntiga = new OrdemServico(cliente.Id, veiculo.Id, emExecucao.Id, "Execucao antiga");
            var execucaoNova = new OrdemServico(cliente.Id, veiculo.Id, emExecucao.Id, "Execucao nova");
            var aguardando = new OrdemServico(cliente.Id, veiculo.Id, aguardandoAprovacao.Id);
            var emDiagnostico = new OrdemServico(cliente.Id, veiculo.Id, diagnostico.Id);
            var ordemRecebida = new OrdemServico(cliente.Id, veiculo.Id, recebida.Id);
            var ordemCancelada = new OrdemServico(cliente.Id, veiculo.Id, cancelada.Id);
            var ordemFinalizada = new OrdemServico(cliente.Id, veiculo.Id, finalizada.Id);
            var ordemEntregue = new OrdemServico(cliente.Id, veiculo.Id, entregue.Id);

            database.Context.OrdensServico.AddRange(
                execucaoNova,
                ordemEntregue,
                ordemRecebida,
                ordemFinalizada,
                emDiagnostico,
                aguardando,
                execucaoAntiga,
                ordemCancelada);

            database.Context.Entry(execucaoAntiga).Property(o => o.DataAbertura).CurrentValue = agora.AddDays(-2);
            database.Context.Entry(execucaoNova).Property(o => o.DataAbertura).CurrentValue = agora.AddDays(-1);
            await database.Context.SaveChangesAsync();
            database.Context.ChangeTracker.Clear();

            var repository = new OrdemServicoRepository(database.Context);

            IReadOnlyCollection<OrdemServico> ordens = await repository.ObterTodos();

            ordens.Select(o => o.Id).Should().Equal(
                execucaoAntiga.Id,
                execucaoNova.Id,
                aguardando.Id,
                emDiagnostico.Id,
                ordemRecebida.Id,
                ordemCancelada.Id);
            ordens.Should().NotContain(o => o.Id == ordemFinalizada.Id || o.Id == ordemEntregue.Id);
        }

        private static async Task<TestDatabase> CreateDatabase()
        {
            var postgreSqlContainer = new PostgreSqlBuilder("postgres:16-alpine").Build();
            await postgreSqlContainer.StartAsync();

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseNpgsql(postgreSqlContainer.GetConnectionString())
                .Options;

            var context = new ApplicationDbContext(options);
            await context.Database.MigrateAsync();

            return new TestDatabase(postgreSqlContainer, context);
        }

        private sealed class TestDatabase(PostgreSqlContainer postgreSqlContainer, ApplicationDbContext context) : IAsyncDisposable
        {
            private readonly PostgreSqlContainer _postgreSqlContainer = postgreSqlContainer;

            public ApplicationDbContext Context { get; } = context;

            public async ValueTask DisposeAsync()
            {
                await Context.DisposeAsync();
                await _postgreSqlContainer.DisposeAsync();
            }
        }
    }
}
