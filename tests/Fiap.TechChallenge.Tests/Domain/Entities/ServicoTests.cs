using Fiap.TechChallenge.Domain.Entities;

namespace Fiap.TechChallenge.Tests.Fiap.TechChallenge.Domain.Entities
{
    public class ServicoTests
    {
        [Fact]
        public void Construtor_DadosValidos_DeveCriarServico()
        {
            var servico = new Servico("Troca de Oleo", "Troca completa de oleo", 120.75m);

            Assert.NotEqual(Guid.Empty, servico.Id);
            Assert.Equal("Troca de Oleo", servico.Nome.Valor);
            Assert.Equal("Troca completa de oleo", servico.Descricao.Valor);
            Assert.Equal(120.75m, servico.ValorUnitario.Valor);
        }

        [Fact]
        public void Atualizar_DeveAtualizarTodosOsCamposETimestamp()
        {
            var servico = new Servico("Alinhamento", "Alinhamento dianteiro", 80m);
            DateTime atualizadoEmOriginal = servico.AtualizadoEm;

            Thread.Sleep(5);
            servico.Atualizar("Alinhamento e Balanceamento", "Alinhamento e Balanceamento", 90m, 70);

            Assert.Equal("Alinhamento e Balanceamento", servico.Nome.Valor);
            Assert.Equal("Alinhamento e Balanceamento", servico.Descricao.Valor);
            Assert.Equal(90m, servico.ValorUnitario.Valor);
            Assert.Equal(70, servico.TempoEstimadoMinutos);
            Assert.True(servico.AtualizadoEm > atualizadoEmOriginal);
        }

        [Fact]
        public void MarcarComoApagado_DevePreencherApagadoEm()
        {
            var servico = new Servico("Revisao", "Revisao preventiva", 250m);

            servico.MarcarComoApagado();

            Assert.NotNull(servico.ApagadoEm);
        }
    }
}
