using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace FIAP.PosTech.Hackathon.Application.Boundaries.Account;

[ExcludeFromCodeCoverage]
public class PatientOutput
{
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("cpf")]
    public required string DocumentNumber { get; set; }

    [JsonPropertyName("email")]
    public required string Email { get; set; }

    [JsonPropertyName("criação")]
    public required DateTime CreatedAt { get; set; }
}