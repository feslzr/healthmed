using System.Diagnostics.CodeAnalysis;

namespace FIAP.PosTech.Hackathon.Application.Boundaries.Customer;

[ExcludeFromCodeCoverage]
public class MedicalServiceOutput
{
    public int ScheduleId { get; set; }

    public required string DoctorName { get; set; }

    public int SpecialtyId { get; set; }

    public required string SpecialtyDescription { get; set; }

    public int? AppointmentStatusId { get; set; }

    public string? AppointmentStatusDescription { get; set; }

    public decimal Price { get; set; }

    public DateTime Datetime { get; set; }
}