using Fiap.TechChallenge.Domain.Entities;

namespace Fiap.TechChallenge.Tests.Fiap.TechChallenge.Domain.Entities
{
    public class ClienteTests
    {
        [Fact]
        public void Construtor_DadosValidos_DeveCriarCliente()
        {
            var cliente = new Cliente("Joao Silva", "52998224725", "joao@email.com", "11999999999");

            Assert.NotEqual(Guid.Empty, cliente.Id);
            Assert.Equal("Joao Silva", cliente.Nome.Valor);
            Assert.Equal("52998224725", cliente.CpfCnpj.Valor);
            Assert.Equal("joao@email.com", cliente.Email.Endereco);
            Assert.Equal("11999999999", cliente.Celular.Numero);
        }

        [Fact]
        public void Atualizar_DeveAtualizarCamposETimestamp()
        {
            var cliente = new Cliente("Joao Silva", "52998224725", "joao@email.com", "11999999999");
            DateTime atualizadoEmOriginal = cliente.AtualizadoEm;

            Thread.Sleep(5);
            cliente.Atualizar("Maria Silva", "11144477735", "maria@email.com", "11888888888");

            Assert.Equal("Maria Silva", cliente.Nome.Valor);
            Assert.Equal("11144477735", cliente.CpfCnpj.Valor);
            Assert.Equal("maria@email.com", cliente.Email.Endereco);
            Assert.Equal("11888888888", cliente.Celular.Numero);
            Assert.True(cliente.AtualizadoEm > atualizadoEmOriginal);
        }

        [Fact]
        public void AlterarNome_DeveAtualizarNomeETimestamp()
        {
            var cliente = new Cliente("Joao Silva", "52998224725", "joao@email.com", "11999999999");
            DateTime atualizadoEmOriginal = cliente.AtualizadoEm;

            Thread.Sleep(5);
            cliente.AlterarNome("Maria Silva");

            Assert.Equal("Maria Silva", cliente.Nome.Valor);
            Assert.True(cliente.AtualizadoEm > atualizadoEmOriginal);
        }

        [Fact]
        public void AlterarEmail_DeveAtualizarEmailETimestamp()
        {
            var cliente = new Cliente("Joao Silva", "52998224725", "joao@email.com", "11999999999");
            DateTime atualizadoEmOriginal = cliente.AtualizadoEm;

            Thread.Sleep(5);
            cliente.AlterarEmail("maria@email.com");

            Assert.Equal("maria@email.com", cliente.Email.Endereco);
            Assert.True(cliente.AtualizadoEm > atualizadoEmOriginal);
        }

        [Fact]
        public void AlterarCelular_DeveAtualizarCelularETimestamp()
        {
            var cliente = new Cliente("Joao Silva", "52998224725", "joao@email.com", "11999999999");
            DateTime atualizadoEmOriginal = cliente.AtualizadoEm;

            Thread.Sleep(5);
            cliente.AlterarCelular("11888888888");

            Assert.Equal("11888888888", cliente.Celular.Numero);
            Assert.True(cliente.AtualizadoEm > atualizadoEmOriginal);
        }

        [Fact]
        public void MarcarComoApagado_DevePreencherApagadoEm()
        {
            var cliente = new Cliente("Joao Silva", "52998224725", "joao@email.com", "11999999999");

            cliente.MarcarComoApagado();

            Assert.NotNull(cliente.ApagadoEm);
        }
    }
}
