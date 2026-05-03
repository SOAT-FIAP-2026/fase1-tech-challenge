using Fiap.TechChallenge.Api.Configurations;
using Fiap.TechChallenge.Api.Middlewares;

namespace Fiap.TechChallenge.Api
{
    public class Startup(IConfiguration configuration)
    {
        public IConfiguration Configuration { get; } = configuration;

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddDependencyInjection(Configuration);
            services.AddJWTConfig(Configuration);
            services.AddSwaggerConfig();
        }

        public void Configure(WebApplication app, IWebHostEnvironment env)
        {
            app.UseMiddleware<ExceptionHandlingMiddleware>();

            app.UseHttpsRedirection();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "TechChallenge API v1");
            });

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
        }
    }
}