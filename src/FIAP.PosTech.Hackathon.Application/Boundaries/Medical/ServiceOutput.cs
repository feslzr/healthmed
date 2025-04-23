using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace FIAP.PosTech.Hackathon.Application.Boundaries.Medical;

[ExcludeFromCodeCoverage]
public class ServiceOutput
{
    [JsonPropertyName("id")]
    public required int ServiceId { get; set; }

    [JsonPropertyName("especialidade")]
    public required string SpecialtyName { get; set; }

    [JsonPropertyName("preço")]
    public required decimal Price { get; set; }

    [JsonPropertyName("criação")]
    public required DateTime CreatedAt { get; set; }
}