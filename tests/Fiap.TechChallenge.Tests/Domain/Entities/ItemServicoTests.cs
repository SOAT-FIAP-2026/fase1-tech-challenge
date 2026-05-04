using Fiap.TechChallenge.Domain.Entities;

namespace Fiap.TechChallenge.Tests.Fiap.TechChallenge.Domain.Entities
{
    public class ItemServicoTests
    {
        [Fact]
        public void Construtor_DadosValidos_DeveCriarItemServico()
        {
            Guid idOrdemServico = Guid.NewGuid();
            Guid idServico = Guid.NewGuid();

            var item = new ItemServico(idOrdemServico, idServico);

            Assert.NotEqual(Guid.Empty, item.Id);
            Assert.Equal(idOrdemServico, item.IdOrdemServico);
            Assert.Equal(idServico, item.IdServico);
            Assert.Null(item.DataHoraInicio);
            Assert.Null(item.DataHoraFim);
        }

        [Fact]
        public void IniciarServico_QuandoNaoIniciado_DevePreencherDataHoraInicio()
        {
            var item = new ItemServico(Guid.NewGuid(), Guid.NewGuid());

            item.IniciarServico();

            Assert.NotNull(item.DataHoraInicio);
            Assert.Null(item.DataHoraFim);
        }

        [Fact]
        public void IniciarServico_QuandoJaIniciado_NaoDeveAlterarDataHoraInicio()
        {
            var item = new ItemServico(Guid.NewGuid(), Guid.NewGuid());
            item.IniciarServico();
            DateTime? inicioOriginal = item.DataHoraInicio;

            item.IniciarServico();

            Assert.Equal(inicioOriginal, item.DataHoraInicio);
        }

        [Fact]
        public void FinalizarServico_QuandoNaoIniciado_DeveLancarExcecao()
        {
            var item = new ItemServico(Guid.NewGuid(), Guid.NewGuid());

            Assert.Throws<InvalidOperationException>(item.FinalizarServico);
        }

        [Fact]
        public void FinalizarServico_QuandoIniciado_DevePreencherDataHoraFim()
        {
            var item = new ItemServico(Guid.NewGuid(), Guid.NewGuid());
            item.IniciarServico();

            item.FinalizarServico();

            Assert.NotNull(item.DataHoraFim);
        }

        [Fact]
        public void ObterTempoExecutadoMinutos_QuandoNaoIniciado_DeveRetornarNulo()
        {
            var item = new ItemServico(Guid.NewGuid(), Guid.NewGuid());

            int? tempo = item.ObterTempoExecutadoMinutos();

            Assert.Null(tempo);
        }

        [Fact]
        public void ObterTempoExecutadoMinutos_QuandoFinalizado_DeveRetornarTempoNaoNegativo()
        {
            var item = new ItemServico(Guid.NewGuid(), Guid.NewGuid());
            DateTime agora = DateTime.UtcNow;
            typeof(ItemServico).GetProperty(nameof(ItemServico.DataHoraInicio))!.SetValue(item, agora.AddMinutes(-10));
            typeof(ItemServico).GetProperty(nameof(ItemServico.DataHoraFim))!.SetValue(item, agora);

            int? tempo = item.ObterTempoExecutadoMinutos();

            Assert.True(tempo >= 0);
        }
    }
}
