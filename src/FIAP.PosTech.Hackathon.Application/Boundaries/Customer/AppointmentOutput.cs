using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace FIAP.PosTech.Hackathon.Application.Boundaries.Customer;

[ExcludeFromCodeCoverage]
public class AppointmentOutput
{
    [JsonPropertyName("id")]
    public required int AppointmentId { get; set; }

    [JsonPropertyName("horário")]
    public required DateTime Datetime { get; set; }

    [JsonPropertyName("doutor")]
    public required string DoctorName { get; set; }

    [JsonPropertyName("crm")]
    public required string DoctorDocument { get; set; }

    [JsonPropertyName("paciente")]
    public required string PatientName { get; set; }

    [JsonPropertyName("especialidade")]
    public required string SpecialtyName { get; set; }

    [JsonPropertyName("preço")]
    public required decimal Price { get; set; }

    [JsonPropertyName("criação")]
    public required DateTime CreatedAt { get; set; }
}