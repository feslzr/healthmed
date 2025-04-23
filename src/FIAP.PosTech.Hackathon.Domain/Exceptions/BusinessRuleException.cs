using System.Diagnostics.CodeAnalysis;

namespace FIAP.PosTech.Hackathon.Domain.Exceptions;

[ExcludeFromCodeCoverage]
public class BusinessRuleException : Exception
{
    public BusinessRuleException() { }

    public BusinessRuleException(string message) : base(message) { }

    public BusinessRuleException(string message, Exception inner) : base(message, inner) { }
}