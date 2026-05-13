using System.Reflection;
using Microsoft.OpenApi.Models;

namespace Paschoalotto.API.Extensions;

public static class SwaggerExtensions
{
    private const string DocumentName = "v1";

    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc(DocumentName, new OpenApiInfo
            {
                Title = "Paschoalotto API",
                Version = "v1",
                Description = "API do sistema Paschoalotto"
            });

            // Mapear IFormFile
            options.MapType<IFormFile>(() => new OpenApiSchema
            {
                Type = "string",
                Format = "binary"
            });

            // Configurar JWT
            var jwtScheme = new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            };
            options.AddSecurityDefinition("Bearer", jwtScheme);
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                { jwtScheme, Array.Empty<string>() }
            });

            // ⭐ IMPORTANTE: Incluir XML comments dos controllers
            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
            if (File.Exists(xmlPath))
            {
                options.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
            }

            // ⭐ Forçar a inclusão de todos os controllers
            options.DocInclusionPredicate((docName, apiDesc) => true);

            // ⭐ Garantir que actions sem [ApiController] sejam incluídas
            options.SwaggerGeneratorOptions.IgnoreObsoleteActions = false;
            options.SwaggerGeneratorOptions.InferSecuritySchemes = true;

            // ⭐ Usar nome completo para evitar conflitos
            options.CustomSchemaIds(type => type.FullName);
        });

        return services;
    }

    public static IApplicationBuilder UseSwaggerDocumentation(this IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint($"/swagger/{DocumentName}/swagger.json", "Paschoalotto API v1");
            options.RoutePrefix = "swagger";
        });

        return app;
    }
}