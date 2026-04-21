using Fiap.TechChallenge.Domain.Interfaces.Service;
using Fiap.TechChallenge.External.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Fiap.TechChallenge.External.Configurations
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddExternal(this IServiceCollection services)
        {
            services.AddScoped<ITokenService, TokenService>();

            return services;
        }
    }
}
