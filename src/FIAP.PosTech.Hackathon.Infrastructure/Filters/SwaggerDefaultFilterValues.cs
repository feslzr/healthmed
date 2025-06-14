﻿using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace FIAP.PosTech.Hackathon.Infrastructure.Filters;

[ExcludeFromCodeCoverage]
internal class SwaggerDefaultFilterValues : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        ApiDescription apiDescription = context.ApiDescription;

        operation.Deprecated |= apiDescription.IsDeprecated();

        foreach (var responseType in context.ApiDescription.SupportedResponseTypes)
        {
            var responseKey = responseType.IsDefaultResponse ? "default" : responseType.StatusCode.ToString();
            var response = operation.Responses[responseKey];

            foreach (var contentType in response.Content.Keys)
                if (!responseType.ApiResponseFormats.Any(x => x.MediaType == contentType))
                    response.Content.Remove(contentType);
        }

        if (operation.Parameters == null)
            return;

        foreach (var parameter in operation.Parameters)
        {
            ApiParameterDescription description = apiDescription.ParameterDescriptions.First(p => p.Name == parameter.Name);

            parameter.Description ??= description.ModelMetadata?.Description;

            if (parameter.Schema.Default == null && description.DefaultValue != null && description.ModelMetadata != null)
            {
                var json = JsonSerializer.Serialize(description.DefaultValue, description.ModelMetadata.ModelType);

                parameter.Schema.Default = OpenApiAnyFactory.CreateFromJson(json);
            }

            parameter.Required |= description.IsRequired;

            if (description.ModelMetadata?.ModelType == typeof(DateTime) || description.ModelMetadata?.ModelType == typeof(DateTime?))
            {
                parameter.Schema.Type = "string";
                parameter.Schema.Format = "date-time";
            }
        }
    }
}