using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using FIAP.PosTech.Hackathon.Infrastructure.Filters;
using FIAP.PosTech.Hackathon.Infrastructure.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Diagnostics.CodeAnalysis;

namespace FIAP.PosTech.Hackathon.Infrastructure.Extensions;

[ExcludeFromCodeCoverage]
public static class SwaggerExtensions
{
    public static void AddSwagger(this IServiceCollection services, bool addAuthorization = true)
    {
        services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;
        }).AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });

        if (addAuthorization)
        {
            services.AddSwaggerGen(options =>
            {
                options.OperationFilter<SwaggerDefaultFilterValues>();

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Bearer Token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    BearerFormat = "JWT"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new List<string>()
                }});
            });
        } else
        {
            services.AddSwaggerGen(options =>
            {
                options.OperationFilter<SwaggerDefaultFilterValues>();
            });
        }
    }

    public static void UseSwaggerPage(this IApplicationBuilder app, IApiVersionDescriptionProvider provider)
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            foreach (var description in provider.ApiVersionDescriptions.Select(d => d.GroupName))
            {
                options.SwaggerEndpoint($"/swagger/{description}/swagger.json",
                                        description.ToUpperInvariant());
            }
        });
    }
}