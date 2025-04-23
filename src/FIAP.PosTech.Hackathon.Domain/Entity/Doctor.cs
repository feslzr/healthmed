using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace FIAP.PosTech.Hackathon.Domain.Entity;

[ExcludeFromCodeCoverage]
[Table("Doctor", Schema = "Fiap")]
public class Doctor
{
    private readonly string _documentErrorMessage = "O número do CRM não pode ser vazio.";

    public Doctor(int accountId, string documentNumber)
    {
        AccountId = accountId;
        DocumentNumber = documentNumber;
    }

    public static Doctor Create(int accountId, string documentNumber) => new(accountId, documentNumber);

    [Key]
    [Required]
    [Column("Id")]
    public int Id { get; set; }

    [Required]
    [Column("AccountId")]
    public int AccountId { get; set; }

    [Required]
    [Column("DocumentNumber")]
    private string _documentNumber = string.Empty;
    public string DocumentNumber
    {
        get => _documentNumber;
        private set => SetDocument(value);
    }

    #region Public Methods

    public void SetDocument(string document)
    {
        if (string.IsNullOrWhiteSpace(document))
            throw new ArgumentException(_documentErrorMessage);

        _documentNumber = document;
    }

    #endregion
}