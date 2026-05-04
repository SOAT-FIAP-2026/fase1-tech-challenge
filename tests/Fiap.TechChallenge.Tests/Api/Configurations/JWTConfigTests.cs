using Fiap.TechChallenge.Api.Configurations;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Xunit;

namespace Fiap.TechChallenge.Tests.Fiap.TechChallenge.Api.Configurations
{
    public class JWTConfigTests
    {
        [Fact]
        public void AddJWTConfig_DeveConfigurarJWTCorretamente()
        {
            // Arrange
            var services = new ServiceCollection();
            var secretKey = "chave_secreta_com_tamanho_suficiente_123";
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "Jwt:Secret", secretKey },
                    { "Jwt:Issuer", "test_issuer" },
                    { "Jwt:Audience", "test_audience" }
                })
                .Build();

            // Act 
            services.AddJWTConfig(configuration);

            var serviceProvider = services.BuildServiceProvider();

            // Assert
            var authOptions = serviceProvider.GetRequiredService<IOptions<AuthenticationOptions>>().Value;
            authOptions.DefaultAuthenticateScheme.Should().Be(JwtBearerDefaults.AuthenticationScheme);
            authOptions.DefaultChallengeScheme.Should().Be(JwtBearerDefaults.AuthenticationScheme);

            var jwtOptions = serviceProvider.GetRequiredService<IOptionsMonitor<JwtBearerOptions>>()
                                           .Get(JwtBearerDefaults.AuthenticationScheme);

            jwtOptions.TokenValidationParameters.Should().NotBeNull();
            jwtOptions.TokenValidationParameters.ValidIssuer.Should().Be("test_issuer");
            jwtOptions.TokenValidationParameters.ValidAudience.Should().Be("test_audience");
            jwtOptions.RequireHttpsMetadata.Should().BeTrue();
            jwtOptions.SaveToken.Should().BeTrue();

            services.Any(x => x.ServiceType == typeof(Microsoft.AspNetCore.Authorization.IAuthorizationService))
                    .Should().BeTrue();
        }
    }
}