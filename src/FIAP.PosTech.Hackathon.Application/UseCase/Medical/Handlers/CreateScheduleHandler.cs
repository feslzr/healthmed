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
public class CreateScheduleUseCase : IRequest<ScheduleOutput>
{
    [BindProperty(Name = "Especialidade")]
    [Required(ErrorMessage = "O campo 'Especialidade' é obrigatório")]
    public required MedicalSpecialtyEnum Specialty { get; set; }

    [BindProperty(Name = "Horário")]
    [Required(ErrorMessage = "O campo 'Horário' é obrigatório")]
    public required DateTime Datetime { get; set; }
}

public class CreateScheduleHandler(ILoggedUser loggedUser,
                                   IDoctorRepository doctorRepository,
                                   IServiceRepository serviceRepository,
                                   IScheduleRepository scheduleRepository) : IRequestHandler<CreateScheduleUseCase, ScheduleOutput>
{
    public async Task<ScheduleOutput> Handle(CreateScheduleUseCase request, CancellationToken cancellationToken)
    {
        var errorMessage = "Não é possível realizar essa solicitação";

        if (!loggedUser.IsDoctorAccess())
            throw new BusinessRuleException(errorMessage);

        if (request.Datetime < DateTime.Now)
            throw new BusinessRuleException("Não é possível criar uma agenda com data menor que o dia atual");

        var login = loggedUser.GetUser();

        var doctor = await doctorRepository.GetByDocumentNumberAsync(login) ?? throw new BusinessRuleException(errorMessage);
        var service = await serviceRepository.GetByDoctorIdAndSpecialtyAsync(doctor.Id, (int)request.Specialty);

        var schedule = new Schedule(service.Id, request.Datetime);

        await scheduleRepository.AddAsync(schedule);
        await scheduleRepository.SaveChangesAsync();

        return new()
        {
            ScheduleId = schedule.Id,
            Datetime = schedule.Datetime,
            Price = service.Price,
            SpecialtyName = ((MedicalSpecialtyEnum)service.SpecialtyId).GetDescription(),
            CreatedAt = schedule.CreatedAt
        };
    }
}