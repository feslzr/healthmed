using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace FIAP.PosTech.Hackathon.Domain.Entity;

[ExcludeFromCodeCoverage]
[Table("Patient", Schema = "Fiap")]
public class Patient
{
    private readonly string _documentErrorMessage = "O número do CPF não pode ser vazio.";
    private readonly string _emailErrorMessage = "Email inválido.";

    public Patient(int accountId, string documentNumber, string email)
    {
        AccountId = accountId;
        DocumentNumber = documentNumber;
        Email = email;
    }

    public static Patient Create(int accountId, string documentNumber, string email) => new(accountId, documentNumber, email);

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

    [Required]
    [Column("Email")]
    private string _email = string.Empty;
    public string Email
    {
        get => _email;
        private set => SetEmail(value);
    }

    #region Public Methods

    public void SetDocument(string document)
    {
        if (string.IsNullOrWhiteSpace(document))
            throw new ArgumentException(_documentErrorMessage);

        _documentNumber = document;
    }

    public void SetEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
            throw new ArgumentException(_emailErrorMessage);

        _email = email;
    }

    #endregion
}