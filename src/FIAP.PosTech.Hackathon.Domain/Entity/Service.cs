using FIAP.PosTech.Hackathon.Domain.Contracts;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace FIAP.PosTech.Hackathon.Domain.Entity;

[ExcludeFromCodeCoverage]
[Table("Service", Schema = "Fiap")]
public class Service : IAuditableEntity
{
    private readonly string _priceErrorMessage = "O preço precisa ser informado.";

    public Service(int doctorId, int specialtyId, decimal price)
    {
        DoctorId = doctorId;
        SpecialtyId = specialtyId;
        Price = price;
    }

    public static Service Create(int doctorId, int specialtyId, decimal price)
        => new(doctorId, specialtyId, price);

    [Key]
    [Required]
    [Column("Id")]
    public int Id { get; set; }

    [Required]
    [Column("DoctorId")]
    public int DoctorId { get; set; }

    [Required]
    [Column("SpecialtyId")]
    public int SpecialtyId { get; set; }

    [Required]
    [Column("Price")]
    private decimal _price;
    public decimal Price
    {
        get => _price;
        private set => SetPrice(value);
    }

    [Required]
    [Column("CreatedAt")]
    public DateTime CreatedAt { get; set; }

    [Required]
    [Column("UpdatedAt")]
    public DateTime UpdatedAt { get; set; }

    #region Public Methods

    public void SetPrice(decimal price)
    {
        if (price == 0)
            throw new ArgumentException(_priceErrorMessage);

        _price = price;
    }

    #endregion
}