using System;
using System.Collections.Generic;
using Fiap.TechChallenge.External.Configurations;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Fiap.TechChallenge.Tests.External.Configurations
{
    /// <summary>
    /// Os testes desta classe alteram a variável de ambiente JWT_SECRET, que é
    /// global ao processo. O xUnit executa os testes de uma mesma classe em
    /// sequência, e o valor original é restaurado no Dispose.
    /// </summary>
    public class JwtSecretResolverTests : IDisposable
    {
        private const string ChaveValida = "chave_secreta_com_tamanho_suficiente_123";
        private const string ChaveDoAmbiente = "chave_vinda_da_variavel_de_ambiente_1234";

        private readonly string? _valorOriginal =
            Environment.GetEnvironmentVariable(JwtSecretResolver.EnvironmentVariableName);

        private static IConfiguration ConfiguracaoCom(string? secret)
        {
            var valores = new Dictionary<string, string?>();

            if (secret is not null)
                valores[JwtSecretResolver.ConfigurationKey] = secret;

            return new ConfigurationBuilder().AddInMemoryCollection(valores).Build();
        }

        private static void DefinirVariavelDeAmbiente(string? valor) =>
            Environment.SetEnvironmentVariable(JwtSecretResolver.EnvironmentVariableName, valor);

        [Fact]
        public void Resolve_ComVariavelDeAmbiente_TemPrecedenciaSobreConfiguracao()
        {
            DefinirVariavelDeAmbiente(ChaveDoAmbiente);

            JwtSecretResolver.Resolve(ConfiguracaoCom(ChaveValida)).Should().Be(ChaveDoAmbiente);
        }

        [Fact]
        public void Resolve_SemVariavelDeAmbiente_UsaConfiguracao()
        {
            DefinirVariavelDeAmbiente(null);

            JwtSecretResolver.Resolve(ConfiguracaoCom(ChaveValida)).Should().Be(ChaveValida);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Resolve_SemNenhumaFonte_LancaInvalidOperationException(string? valorDaConfiguracao)
        {
            DefinirVariavelDeAmbiente(null);

            Action acao = () => JwtSecretResolver.Resolve(ConfiguracaoCom(valorDaConfiguracao));

            acao.Should().Throw<InvalidOperationException>()
                .WithMessage("*não configurada*");
        }

        [Fact]
        public void Resolve_ComChaveCurta_LancaInvalidOperationException()
        {
            DefinirVariavelDeAmbiente(null);

            Action acao = () => JwtSecretResolver.Resolve(ConfiguracaoCom("curta"));

            acao.Should().Throw<InvalidOperationException>()
                .WithMessage($"*{JwtSecretResolver.MinimumLength} caracteres*");
        }

        [Fact]
        public void Resolve_ComVariavelDeAmbienteEmBranco_CaiParaConfiguracao()
        {
            DefinirVariavelDeAmbiente("   ");

            JwtSecretResolver.Resolve(ConfiguracaoCom(ChaveValida)).Should().Be(ChaveValida);
        }

        [Fact]
        public void Resolve_SemConfiguracao_LancaArgumentNullException()
        {
            Action acao = () => JwtSecretResolver.Resolve(null!);

            acao.Should().Throw<ArgumentNullException>();
        }

        public void Dispose()
        {
            DefinirVariavelDeAmbiente(_valorOriginal);
            GC.SuppressFinalize(this);
        }
    }
}
