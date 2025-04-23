using System.Diagnostics.CodeAnalysis;

namespace FIAP.PosTech.Hackathon.Domain.Exceptions;
[ExcludeFromCodeCoverage]
public class NullEntityException : Exception
{
    public NullEntityException()
    {
    }

    public NullEntityException(string message)
        : base(message)
    {
    }

    public NullEntityException(string message, Exception inner)
        : base(message, inner)
    {
    }
}