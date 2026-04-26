using Fiap.TechChallenge.Application.Interfaces;
using Fiap.TechChallenge.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Fiap.TechChallenge.Application.Configurations
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IAutenticacaoService, AutenticacaoService>();
            services.AddScoped<IServicoService, ServicoService>();

            return services;
        }
    }
}
