namespace Fiap.TechChallenge.Domain.Entities
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
    }
}
