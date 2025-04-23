namespace FIAP.PosTech.Hackathon.Domain.Contracts;

public interface IAuditableEntity
{
    DateTime CreatedAt { get; set; }
    DateTime UpdatedAt { get; set; }
}