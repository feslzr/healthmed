using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace FIAP.PosTech.Hackathon.Application.Boundaries.Medical;

[ExcludeFromCodeCoverage]
public class ScheduleOutput
{
    [JsonPropertyName("id")]
    public required int ScheduleId { get; set; }

    [JsonPropertyName("horário")]
    public required DateTime Datetime { get; set; }

    [JsonPropertyName("especialidade")]
    public required string SpecialtyName { get; set; }

    [JsonPropertyName("preço")]
    public required decimal Price { get; set; }

    [JsonPropertyName("criação")]
    public required DateTime CreatedAt { get; set; }
}
