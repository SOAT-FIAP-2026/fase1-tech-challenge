using Fiap.TechChallenge.Domain.ValueObjects;

namespace Fiap.TechChallenge.Tests.Fiap.TechChallenge.Domain.ValueObjects
{
    public class DescricaoVOTests
    {
        [Fact]
        public void Construtor_DescricaoValida_DeveCriarObjetoComTrim()
        {
            string descricaoComEspacos = "  Revisão completa  ";

            DescricaoVO descricaoVO = new(descricaoComEspacos);

            Assert.Equal("Revisão completa", descricaoVO.Valor);
        }

        [Fact]
        public void Construtor_DescricaoVazia_DeveLancarExcecao()
        {
            Assert.Throws<ArgumentException>(() => new DescricaoVO(""));
            Assert.Throws<ArgumentException>(() => new DescricaoVO("   "));
        }

        [Fact]
        public void Construtor_DescricaoMaiorQueLimite_DeveLancarExcecao()
        {
            string descricaoGrandeDemais = new('a', 256);

            var exception = Assert.Throws<ArgumentException>(() => new DescricaoVO(descricaoGrandeDemais));

            Assert.Equal("A descrição não pode exceder 255 caracteres.", exception.Message);
        }

        [Fact]
        public void Contains_DeveBuscarSemDiferenciarMaiusculasEMinusculas()
        {
            DescricaoVO descricaoVO = new("Revisão completa do veículo");

            Assert.True(descricaoVO.Contains("COMPLETA"));
            Assert.True(descricaoVO.Contains("veículo"));
            Assert.False(descricaoVO.Contains("troca de óleo"));
        }
    }
}