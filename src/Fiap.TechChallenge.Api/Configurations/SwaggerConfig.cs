using Fiap.TechChallenge.Application.Configurations;
using Fiap.TechChallenge.External.Configurations;
using Fiap.TechChallenge.Infrastructure.Configurations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace Fiap.TechChallenge.Api.Configurations
{
    public static class SwaggerConfig
    {
        public static void AddSwaggerConfig(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                // Configuração básica do documento
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "TechChallenge API", Version = "v1" });

                // 1. Adiciona o botão "Authorize" e define o formato esperado
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Insira o token JWT abaixo."
                });

                // 2. Obriga o Swagger a enviar o token no cabeçalho de todas as requisições
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            Array.Empty<string>()
                        }
                    });
                });
        }
    }
}