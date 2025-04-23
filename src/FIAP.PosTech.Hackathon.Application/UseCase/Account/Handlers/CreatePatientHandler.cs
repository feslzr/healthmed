using FIAP.PosTech.Hackathon.Application.Boundaries.Account;
using FIAP.PosTech.Hackathon.Application.Interfaces.Repository;
using FIAP.PosTech.Hackathon.Application.UseCase.Account.Base;
using FIAP.PosTech.Hackathon.Domain.Entity;
using FIAP.PosTech.Hackathon.Domain.Exceptions;
using FIAP.PosTech.Hackathon.Domain.Helpers;
using MediatR;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace FIAP.PosTech.Hackathon.Application.UseCase.Account.Handlers;

[ExcludeFromCodeCoverage]
public class CreatePatientUseCase : BaseAccountUseCase, IRequest<PatientOutput>
{
    [Required(ErrorMessage = "O campo 'Nome' é obrigatório")]
    public override string? Name { get; set; }

    [Required(ErrorMessage = "O campo 'Senha' é obrigatório")]
    public override string? Password { get; set; }

    [JsonPropertyName("cpf")]
    [Required(ErrorMessage = "O campo 'CPF' é obrigatório")]
    public override string? DocumentNumber { get; set; }

    [JsonPropertyName("email")]
    [Required(ErrorMessage = "O campo 'E-mail' é obrigatório")]
    [EmailAddress(ErrorMessage = EmailErrorMessage)]
    public virtual string? Email { get; set; }
}

public class CreatePatientHandler(IAccountRepository accountRepository, IPatientRepository patientRepository) : IRequestHandler<CreatePatientUseCase, PatientOutput>
{
    public async Task<PatientOutput> Handle(CreatePatientUseCase request, CancellationToken cancellationToken)
    {
        var patient = await patientRepository.GetByDocumentNumberOrEmailAsync(request.DocumentNumber!, request.Email!);

        if (patient != null)
            throw new BusinessRuleException("Já existe uma conta cadastrada com esse CPF ou e-mail");

        var password = PasswordHelper.HashPassword(request.Password!);

        var account = new Domain.Entity.Account(request.Name!, password);

        await accountRepository.AddAsync(account);
        await accountRepository.SaveChangesAsync();

        patient = new Patient(account.Id, request.DocumentNumber!, request.Email!);

        await patientRepository.AddAsync(patient);
        await patientRepository.SaveChangesAsync();

        return new()
        {
            Name = account.Name,
            DocumentNumber = patient.DocumentNumber,
            Email = patient.Email,
            CreatedAt = account.CreatedAt
        };
    }
}