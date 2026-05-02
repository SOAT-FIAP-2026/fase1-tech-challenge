using Fiap.TechChallenge.Domain.Entities;

namespace Fiap.TechChallenge.Tests.Fiap.TechChallenge.Domain.Entities
{
    public class OrdemServicoTests
    {
        [Fact]
        public void Construtor_DadosValidos_DeveCriarOrdemServico()
        {
            var ordemServico = new OrdemServico(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Observacao");

            Assert.NotEqual(Guid.Empty, ordemServico.Id);
            Assert.Equal("Observacao", ordemServico.Observacao);
            Assert.NotEqual(default, ordemServico.DataAbertura);
        }

        [Fact]
        public void DefinirOrcamento_DeveAssociarOrcamentoAOrdem()
        {
            var ordemServico = new OrdemServico(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
            var orcamento = new Orcamento(ordemServico.Id, 123.45m);

            ordemServico.DefinirOrcamento(orcamento);

            Assert.Same(orcamento, ordemServico.Orcamento);
        }

        [Fact]
        public void RecalcularOrcamento_DeveSomarServicosEpecas()
        {
            var ordemServico = new OrdemServico(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
            var servicos = new[]
            {
                new Servico("Troca de Oleo", "Troca completa", 100m),
                new Servico("Alinhamento", "Alinhamento dianteiro", 50m)
            };
            var pecas = new[]
            {
                new PecaInsumo("Filtro de Ar", 25m)
            };

            ordemServico.RecalcularOrcamento(servicos, pecas);

            Assert.NotNull(ordemServico.Orcamento);
            Assert.Equal(175m, ordemServico.Orcamento!.ValorTotal.Valor);
        }

        [Fact]
        public void RecalcularOrcamento_DeveAtualizarOrcamentoExistente()
        {
            var ordemServico = new OrdemServico(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
            ordemServico.DefinirOrcamento(new Orcamento(ordemServico.Id, 100m));

            var servicos = new[]
            {
                new Servico("Troca de Oleo", "Troca completa", 120m)
            };
            var pecas = new[]
            {
                new PecaInsumo("Filtro de Ar", 35m)
            };

            ordemServico.RecalcularOrcamento(servicos, pecas);

            Assert.Equal(155m, ordemServico.Orcamento!.ValorTotal.Valor);
        }

        [Fact]
        public void SincronizarItens_DeveAdicionarItensNovosERecalcularOrcamento()
        {
            var ordemServico = new OrdemServico(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
            var servicoExistente = new Servico("Troca de Oleo", "Troca completa", 100m);
            var pecaExistente = new PecaInsumo("Filtro de Ar", 25m);

            ordemServico.SincronizarItens([servicoExistente], [pecaExistente]);

            var servicoNovo = new Servico("Alinhamento", "Alinhamento dianteiro", 50m);
            var pecaNova = new PecaInsumo("Oleo Motor", 30m);

            ordemServico.SincronizarItens([servicoExistente, servicoNovo], [pecaExistente, pecaNova]);

            Assert.Equal(2, ordemServico.ItensServico.Count);
            Assert.Equal(2, ordemServico.ItensPecaInsumo.Count);
            Assert.Equal(205m, ordemServico.Orcamento!.ValorTotal.Valor);
        }
    }
}
