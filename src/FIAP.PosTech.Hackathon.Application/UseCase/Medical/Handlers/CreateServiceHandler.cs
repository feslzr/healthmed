using FIAP.PosTech.Hackathon.Application.Boundaries.Medical;
using FIAP.PosTech.Hackathon.Application.Interfaces.Repository;
using FIAP.PosTech.Hackathon.Domain.Contracts;
using FIAP.PosTech.Hackathon.Domain.Entity;
using FIAP.PosTech.Hackathon.Domain.Enums;
using FIAP.PosTech.Hackathon.Domain.Exceptions;
using FIAP.PosTech.Hackathon.Domain.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace FIAP.PosTech.Hackathon.Application.UseCase.Medical.Handlers;

[ExcludeFromCodeCoverage]
public class CreateServiceUseCase : IRequest<ServiceOutput>
{
    [BindProperty(Name = "Especialidade")]
    [Required(ErrorMessage = "O campo 'Especialidade' é obrigatório")]
    public required MedicalSpecialtyEnum Specialty { get; set; }

    [BindProperty(Name = "Valor")]
    [Required(ErrorMessage = "O campo 'Valor' é obrigatório")]
    public required decimal Price { get; set; }
}

public class CreateServiceHandler(ILoggedUser loggedUser, IDoctorRepository doctorRepository, IServiceRepository serviceRepository) : IRequestHandler<CreateServiceUseCase, ServiceOutput>
{
    public async Task<ServiceOutput> Handle(CreateServiceUseCase request, CancellationToken cancellationToken)
    {
        var errorMessage = "Não é possível realizar essa solicitação";

        if (!loggedUser.IsDoctorAccess())
            throw new BusinessRuleException(errorMessage);

        var login = loggedUser.GetUser();

        var doctor = await doctorRepository.GetByDocumentNumberAsync(login) ??
                     throw new BusinessRuleException(errorMessage);

        var exitingService = await serviceRepository.GetByDoctorIdAndSpecialtyNullableAsync(doctor.Id, (int)request.Specialty);

        if (exitingService != null)
            throw new BusinessRuleException("Já existe um serviço dessa especialidade para esse médico");

        var service = new Service(doctor.Id, (int)request.Specialty, request.Price);

        await serviceRepository.AddAsync(service);
        await serviceRepository.SaveChangesAsync();

        return new()
        {
            ServiceId = service.Id,
            Price = service.Price,
            SpecialtyName = ((MedicalSpecialtyEnum)service.SpecialtyId).GetDescription(),
            CreatedAt = service.CreatedAt
        };
    }
}