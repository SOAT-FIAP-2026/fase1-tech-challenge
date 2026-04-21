using Fiap.TechChallenge.Infrastructure.Configurations;
using Fiap.TechChallenge.Application.Configurations;
using Fiap.TechChallenge.External.Configurations;

namespace Fiap.TechChallenge.Api.Configurations
{
    public static class DependencyInjectionConfig
    {
        public static void AddDependencyInjection(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddInfrastructure(configuration);
            services.AddApplication();
            services.AddExternal();

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
        }
    }
}