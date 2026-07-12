using Moq;
using System.Data.Common;
using Fiap.TechChallenge.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Testcontainers.PostgreSql;
using Xunit;

namespace Fiap.TechChallenge.Tests.Api.Support
{
    public sealed class ApiWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
    {
        private readonly PostgreSqlContainer _postgreSqlContainer = new PostgreSqlBuilder("postgres:16-alpine").Build();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");

            builder.ConfigureServices(services =>
            {
                services.RemoveAll<DbContextOptions<ApplicationDbContext>>();

                services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
                    options.UseNpgsql(_postgreSqlContainer.GetConnectionString()));

                // Mock IEmailService to prevent actual emails from being sent during tests
                var emailServiceMock = new Moq.Mock<global::Fiap.TechChallenge.Domain.Interfaces.Service.IEmailService>();
                emailServiceMock
                    .Setup(e => e.EnviarEmailAsync(Moq.It.IsAny<string>(), Moq.It.IsAny<string>(), Moq.It.IsAny<string>()))
                    .ReturnsAsync(true);
                
                services.RemoveAll<global::Fiap.TechChallenge.Domain.Interfaces.Service.IEmailService>();
                services.AddSingleton(emailServiceMock.Object);
            });
        }

        public async Task InitializeAsync()
        {
            await _postgreSqlContainer.StartAsync();
        }

        async Task IAsyncLifetime.DisposeAsync()
        {
            Dispose();
            await _postgreSqlContainer.DisposeAsync();
        }
    }
}
