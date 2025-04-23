using FIAP.PosTech.Hackathon.Application.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace FIAP.PosTech.Hackathon.API.Filters;

[ExcludeFromCodeCoverage]
public class ExceptionFilterAttribute() : IAsyncExceptionFilter
{
    public Task OnExceptionAsync(ExceptionContext context)
    {
        context.Result = new JsonResult(new ErrorResponse(context.Exception.Message))
        {
            StatusCode = (int)HttpStatusCode.InternalServerError
        };

        context.ExceptionHandled = true;
        return Task.CompletedTask;
    }
}