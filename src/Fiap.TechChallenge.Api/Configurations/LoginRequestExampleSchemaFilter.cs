using Fiap.TechChallenge.Application.DTOs.Requests;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Fiap.TechChallenge.Api.Configurations
{
    public sealed class LoginRequestExampleSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (context.Type != typeof(LoginRequest))
            {
                return;
            }

            schema.Example = new OpenApiObject
            {
                ["login"] = new OpenApiString("admin@techchallenge.com"),
                ["senha"] = new OpenApiString("Admin@123")
            };
        }
    }
}