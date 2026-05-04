using Fiap.TechChallenge.Domain.Entities;

namespace Fiap.TechChallenge.Tests.Fiap.TechChallenge.Domain.Entities
{
    public class EstoqueTests
    {
        [Fact]
        public void Construtor_QuantidadeValida_DeveCriarEstoque()
        {
            Guid idPecaInsumo = Guid.NewGuid();

            var estoque = new Estoque(idPecaInsumo, 10);

            Assert.NotEqual(Guid.Empty, estoque.Id);
            Assert.Equal(idPecaInsumo, estoque.IdPecaInsumo);
            Assert.Equal(10, estoque.Quantidade);
        }

        [Fact]
        public void Construtor_QuantidadeNegativa_DeveLancarExcecao()
        {
            Assert.Throws<ArgumentException>(() => new Estoque(Guid.NewGuid(), -1));
        }

        [Fact]
        public void AdicionarQuantidade_QuantidadePositiva_DeveIncrementarEstoque()
        {
            var estoque = new Estoque(Guid.NewGuid(), 10);
            DateTime atualizadoEmOriginal = estoque.AtualizadoEm;

            estoque.AdicionarQuantidade(5);

            Assert.Equal(15, estoque.Quantidade);
            Assert.True(estoque.AtualizadoEm >= atualizadoEmOriginal);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void AdicionarQuantidade_QuantidadeNaoPositiva_DeveLancarExcecao(int quantidade)
        {
            var estoque = new Estoque(Guid.NewGuid(), 10);

            Assert.Throws<ArgumentException>(() => estoque.AdicionarQuantidade(quantidade));
        }

        [Fact]
        public void RemoverQuantidade_QuantidadeDisponivel_DeveDecrementarEstoque()
        {
            var estoque = new Estoque(Guid.NewGuid(), 10);
            DateTime atualizadoEmOriginal = estoque.AtualizadoEm;

            estoque.RemoverQuantidade(4);

            Assert.Equal(6, estoque.Quantidade);
            Assert.True(estoque.AtualizadoEm >= atualizadoEmOriginal);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void RemoverQuantidade_QuantidadeNaoPositiva_DeveLancarExcecao(int quantidade)
        {
            var estoque = new Estoque(Guid.NewGuid(), 10);

            Assert.Throws<ArgumentException>(() => estoque.RemoverQuantidade(quantidade));
        }

        [Fact]
        public void RemoverQuantidade_QuantidadeMaiorQueEstoque_DeveLancarExcecao()
        {
            var estoque = new Estoque(Guid.NewGuid(), 10);

            Assert.Throws<InvalidOperationException>(() => estoque.RemoverQuantidade(11));
        }
    }
}
