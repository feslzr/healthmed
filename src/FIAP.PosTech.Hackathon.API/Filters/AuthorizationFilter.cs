using FIAP.PosTech.Hackathon.Domain.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics.CodeAnalysis;

namespace FIAP.PosTech.Hackathon.API.Filters;

[ExcludeFromCodeCoverage]
public class AuthorizationFilter(ILoggedUser loggedUser) : IAsyncAuthorizationFilter
{
    public Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        if (!loggedUser.Validate() && !IsAnonymousRote(context))
            context.Result = new UnauthorizedResult();

        return Task.CompletedTask;
    }

    private static bool IsAnonymousRote(AuthorizationFilterContext context)
        => context.ActionDescriptor.EndpointMetadata.Any(a => a.GetType() == typeof(AllowAnonymousAttribute));
}