using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Moq;
using Swashbuckle.AspNetCore.SwaggerGen;
using Xunit;

namespace Fiap.TechChallenge.Tests.Fiap.TechChallenge.Api.Configurations
{
    public class SwaggerConfigTests
    {
        [Fact] 
        public void AddSwaggerConfig_DeveConfigurarSwaggerCorretamente()
        {
            // Arrange
            var services = new ServiceCollection();

            var mockEnv = new Mock<IWebHostEnvironment>();
            services.AddSingleton(mockEnv.Object);
            services.AddLogging();

            // Act
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "TechChallenge API", Version = "v1" });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Insira o token JWT abaixo."
                });

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

            var serviceProvider = services.BuildServiceProvider();

            // Assert
            var options = serviceProvider.GetRequiredService<IOptions<SwaggerGeneratorOptions>>().Value;
            options.Should().NotBeNull();

            options.SwaggerDocs.Should().ContainKey("v1");
            options.SecuritySchemes.Should().ContainKey("Bearer");

            var bearerScheme = options.SecuritySchemes["Bearer"];
            bearerScheme.Scheme.Should().Be("Bearer");
            bearerScheme.BearerFormat.Should().Be("JWT");
            bearerScheme.Type.Should().Be(SecuritySchemeType.Http);

            options.SecurityRequirements.Should().NotBeEmpty();
            options.SecurityRequirements.Any(r => r.Keys.Any(k => k.Reference.Id == "Bearer")).Should().BeTrue();
        }
    }
}