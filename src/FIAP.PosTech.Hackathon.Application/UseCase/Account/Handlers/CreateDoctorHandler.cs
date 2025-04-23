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
public class CreateDoctorUseCase : BaseAccountUseCase, IRequest<DoctorOutput>
{
    [Required(ErrorMessage = "O campo 'Nome' é obrigatório")]
    public override string? Name { get; set; }

    [Required(ErrorMessage = "O campo 'Senha' é obrigatório")]
    public override string? Password { get; set; }

    [JsonPropertyName("crm")]
    [Required(ErrorMessage = "O campo 'CRM' é obrigatório")]
    public override string? DocumentNumber { get; set; }
}

public class CreateDoctorHandler(IAccountRepository accountRepository, IDoctorRepository doctorRepository) : IRequestHandler<CreateDoctorUseCase, DoctorOutput>
{
    public async Task<DoctorOutput> Handle(CreateDoctorUseCase request, CancellationToken cancellationToken)
    {
        var doctor = await doctorRepository.GetByDocumentNumberAsync(request.DocumentNumber!);

        if (doctor != null)
            throw new BusinessRuleException("Já existe uma conta cadastrada com esse CRM");

        var password = PasswordHelper.HashPassword(request.Password!);

        var account = new Domain.Entity.Account(request.Name!, password);

        await accountRepository.AddAsync(account);
        await accountRepository.SaveChangesAsync();

        doctor = new Doctor(account.Id, request.DocumentNumber!);

        await doctorRepository.AddAsync(doctor);
        await doctorRepository.SaveChangesAsync();

        return new()
        {
            Name = account.Name,
            DocumentNumber = doctor.DocumentNumber,
            CreatedAt = account.CreatedAt
        };
    }
}