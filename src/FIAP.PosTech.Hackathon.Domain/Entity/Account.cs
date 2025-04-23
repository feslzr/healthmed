using FIAP.PosTech.Hackathon.Domain.Contracts;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace FIAP.PosTech.Hackathon.Domain.Entity;

[ExcludeFromCodeCoverage]
[Table("Account", Schema = "Fiap")]
public class Account : IAuditableEntity
{
    private readonly string _nameErrorMessage = "O nome não pode ser vazio.";
    private readonly string _passwordErrorMessage = "A senha não pode ser vazia.";

    public Account(string name, string password)
    {
        Name = name;
        Password = password;
    }

    public static Account Create(string name, string password)
        => new(name, password);

    [Key]
    [Required]
    [Column("Id")]
    public int Id { get; set; }

    [Required]
    [Column("Name")]
    private string _name = string.Empty;
    public string Name
    {
        get => _name;
        private set => SetName(value);
    }

    [Required]
    [Column("Password")]
    private string _password = string.Empty;
    public string Password
    {
        get => _password;
        private set => SetPassword(value);
    }

    [Required]
    [Column("CreatedAt")]
    public DateTime CreatedAt { get; set; }

    [Required]
    [Column("UpdatedAt")]
    public DateTime UpdatedAt { get; set; }

    #region Public Methods

    public void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(_nameErrorMessage);

        _name = name;
    }

    public void SetPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException(_passwordErrorMessage);

        _password = password;
    }

    #endregion
}