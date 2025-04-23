using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace FIAP.PosTech.Hackathon.Application.Boundaries.Account;

[ExcludeFromCodeCoverage]
public class DoctorOutput
{
    [JsonPropertyName("nome")]
    public required string Name { get; set; }

    [JsonPropertyName("crm")]
    public required string DocumentNumber { get; set; }

    [JsonPropertyName("criação")]
    public required DateTime CreatedAt { get; set; }
}