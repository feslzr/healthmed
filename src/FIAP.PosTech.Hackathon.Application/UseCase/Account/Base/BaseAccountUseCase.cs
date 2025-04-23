using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace FIAP.PosTech.Hackathon.Application.UseCase.Account.Base;

[ExcludeFromCodeCoverage]
public class BaseAccountUseCase
{
    public const string EmailErrorMessage = "O endereço de e-mail fornecido não é válido";

    [JsonPropertyName("nome")]
    [BindProperty(Name = "Nome")]
    public virtual string? Name { get; set; }

    [JsonPropertyName("senha")]
    [BindProperty(Name = "Senha")]
    public virtual string? Password { get; set; }

    [BindProperty(Name = "Documento")]
    public virtual string? DocumentNumber { get; set; }
}