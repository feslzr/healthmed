using FIAP.PosTech.Hackathon.Domain.Contracts;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace FIAP.PosTech.Hackathon.Domain.Entity;

[ExcludeFromCodeCoverage]
[Table("Appointment", Schema = "Fiap")]
public class Appointment(int patientId, int scheduleId, int statusId) : IAuditableEntity
{
    public static Appointment Create(int patientId, int scheduleId, int statusId)
        => new(patientId, scheduleId, statusId);

    [Key]
    [Required]
    [Column("Id")]
    public int Id { get; set; }

    [Required]
    [Column("PatientId")]
    public int PatientId { get; set; } = patientId;

    [Required]
    [Column("ScheduleId")]
    public int ScheduleId { get; set; } = scheduleId;

    [Required]
    [Column("StatusId")]
    public int StatusId { get; set; } = statusId;

    [Column("Justification")]
    public string? Justification { get; set; }

    [Required]
    [Column("CreatedAt")]
    public DateTime CreatedAt { get; set; }

    [Required]
    [Column("UpdatedAt")]
    public DateTime UpdatedAt { get; set; }
}