using Asp.Versioning.ApiExplorer;
using FIAP.PosTech.Hackathon.API.Filters;
using FIAP.PosTech.Hackathon.API.Middleware;
using FIAP.PosTech.Hackathon.Application.DependencyInjectionExtension;
using FIAP.PosTech.Hackathon.Application.Interfaces;
using FIAP.PosTech.Hackathon.Application.Services;
using FIAP.PosTech.Hackathon.Domain.Contracts;
using FIAP.PosTech.Hackathon.Domain.Services;
using FIAP.PosTech.Hackathon.Domain.Settings;
using FIAP.PosTech.Hackathon.Infrastructure.Data.DependencyInjectionExtension;
using FIAP.PosTech.Hackathon.Infrastructure.Extensions;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRouting(options => options.LowercaseUrls = true);

builder.Services.AddControllers(options =>
{
    options.Filters.Add<AuthorizationFilter>();
    options.Filters.Add<ExceptionFilterAttribute>();
});

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwagger();

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(JwtSettings.Settings));

builder.Services.AddScoped<ILoggedUser, LoggedUser>();
builder.Services.AddScoped<IAccessTokenService, AccessTokenService>();

builder.Services.AddInfraConfiguration(builder.Configuration);

builder.Services.AddHttpContextAccessor();
builder.Services.AddApplicationMediatR();

builder.Host.UseDefaultServiceProvider(option => option.ValidateScopes = false);

var app = builder.Build();

app.UseSwaggerPage(app.Services.GetRequiredService<IApiVersionDescriptionProvider>());

app.UseMiddleware<JwtValidationMiddleware>();

#if DEBUG
app.UseDeveloperExceptionPage();
#endif

app.UseHttpsRedirection();
app.MapControllers();

await app.RunAsync();

/// <summary>
/// Configuração inicial da aplicação
/// </summary>
[ExcludeFromCodeCoverage]
public static partial class Program { }