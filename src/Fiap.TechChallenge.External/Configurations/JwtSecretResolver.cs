using Microsoft.Extensions.Configuration;

namespace Fiap.TechChallenge.External.Configurations
{
    /// <summary>
    /// Resolve a chave de assinatura do JWT em um único ponto.
    ///
    /// A variável de ambiente JWT_SECRET tem precedência: é ela que o Secret do
    /// Kubernetes e o docker-compose injetam, e o valor nunca é versionado. A
    /// configuração Jwt:Secret continua aceita para execução local e testes.
    /// Não existe valor padrão: sem nenhuma das duas fontes a aplicação falha ao
    /// subir, em vez de assinar tokens com uma chave previsível.
    /// </summary>
    public static class JwtSecretResolver
    {
        public const string EnvironmentVariableName = "JWT_SECRET";
        public const string ConfigurationKey = "Jwt:Secret";

        /// <summary>Comprimento mínimo aceito para a chave HMAC-SHA256.</summary>
        public const int MinimumLength = 32;

        public static string Resolve(IConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(configuration);

            string? secret = Environment.GetEnvironmentVariable(EnvironmentVariableName);

            if (string.IsNullOrWhiteSpace(secret))
                secret = configuration[ConfigurationKey];

            if (string.IsNullOrWhiteSpace(secret))
            {
                throw new InvalidOperationException(
                    $"Chave de assinatura do JWT não configurada. Defina a variável de ambiente {EnvironmentVariableName} ou a configuração {ConfigurationKey}.");
            }

            if (secret.Length < MinimumLength)
            {
                throw new InvalidOperationException(
                    $"A chave de assinatura do JWT precisa ter pelo menos {MinimumLength} caracteres.");
            }

            return secret;
        }
    }
}
