using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace FIAP.PosTech.Hackathon.Application.DependencyInjectionExtension;

[ExcludeFromCodeCoverage]
public static class ApplicationConfigurationMediatR
{
    public static IServiceCollection AddApplicationMediatR(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        services.AddOptions();

        return services;
    }
}