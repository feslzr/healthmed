using System.Diagnostics.CodeAnalysis;

namespace FIAP.PosTech.Hackathon.Domain.Settings;

[ExcludeFromCodeCoverage]
public class JwtSettings
{
    public const string Settings = "JwtSettings";

    public required string AuthorizationHeader { get; set; }

    public required string Issuer { get; set; }

    public required string Audience { get; set; }

    public required string Secret { get; set; }

    public required double ExpirationMinutes { get; set; }
}