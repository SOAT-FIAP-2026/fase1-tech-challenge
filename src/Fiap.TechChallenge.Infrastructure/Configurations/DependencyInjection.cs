using Fiap.TechChallenge.Domain.Interfaces.Repository;
using Fiap.TechChallenge.Domain.Interfaces.Security;
using Fiap.TechChallenge.Infrastructure.Repositories;
using Fiap.TechChallenge.Infrastructure.Security;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Data;
using System.Data.SqlClient;

namespace Fiap.TechChallenge.Infrastructure.Configurations
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            //services.AddScoped<IDbConnection>(sp =>
            //new Npgsql.NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IDbConnection>(sp => {
                var connection = new SqliteConnection("Data Source=:memory:");
                connection.Open(); 
                return connection;
            });

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            services.AddScoped<ICrypto, Crypto>();

            return services;
        }
    }
}

