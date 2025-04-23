using System.Diagnostics.CodeAnalysis;

namespace FIAP.PosTech.Hackathon.Application.Models;

[ExcludeFromCodeCoverage]
public class ErrorResponse
{
    public ErrorResponse()
    {
        Messages = [];
    }

    public ErrorResponse(string message)
    {
        Messages = [message];
    }

    public ErrorResponse(List<string> messages)
    {
        Messages = messages;
    }

    public IList<string> Messages { get; private set; }
}