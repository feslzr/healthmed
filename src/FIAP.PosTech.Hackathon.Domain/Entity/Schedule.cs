using FIAP.PosTech.Hackathon.Domain.Contracts;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace FIAP.PosTech.Hackathon.Domain.Entity;

[ExcludeFromCodeCoverage]
[Table("Schedule", Schema = "Fiap")]
public class Schedule(int serviceId, DateTime datetime) : IAuditableEntity
{
    public static Schedule Create(int serviceId, DateTime datetime)
        => new(serviceId, datetime);

    [Key]
    [Required]
    [Column("Id")]
    public int Id { get; set; }

    [Required]
    [Column("ServiceId")]
    public int ServiceId { get; set; } = serviceId;

    [Required]
    [Column("Datetime")]
    public DateTime Datetime { get; set; } = datetime;

    [Required]
    [Column("CreatedAt")]
    public DateTime CreatedAt { get; set; }

    [Required]
    [Column("UpdatedAt")]
    public DateTime UpdatedAt { get; set; }
}
