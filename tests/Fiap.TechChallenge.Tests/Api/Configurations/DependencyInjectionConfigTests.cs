using Fiap.TechChallenge.Application.Configurations;
using Fiap.TechChallenge.External.Configurations;
using Fiap.TechChallenge.Infrastructure.Configurations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using FluentAssertions;

namespace Fiap.TechChallenge.Tests.Fiap.TechChallenge.Api.Configurations
{
    public class DependencyInjectionConfigTests
    {
        [Fact] 
        public void AddDependencyInjection_DeveConfigurarServicosCorretamente()
        {
            // Arrange
            IServiceCollection services = new ServiceCollection();

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>())
                .Build();

            // Act
            services.AddInfrastructure(configuration);
            services.AddApplication();
            services.AddExternal();

            var serviceProvider = services.BuildServiceProvider();

            // Assert
            services.Should().NotBeEmpty();
        }
    }
}