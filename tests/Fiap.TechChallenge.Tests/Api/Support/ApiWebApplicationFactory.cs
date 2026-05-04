using System.Data.Common;
using Fiap.TechChallenge.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Fiap.TechChallenge.Tests.Api.Support
{
    public sealed class ApiWebApplicationFactory : WebApplicationFactory<Program>
    {
        private readonly DbConnection _connection;

        public ApiWebApplicationFactory()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");

            builder.ConfigureServices(services =>
            {
                services.RemoveAll<DbContextOptions<ApplicationDbContext>>();
                services.RemoveAll<DbConnection>();

                services.AddSingleton(_connection);
                services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
                    options.UseSqlite(serviceProvider.GetRequiredService<DbConnection>()));
            });
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
                _connection.Dispose();
        }
    }
}
