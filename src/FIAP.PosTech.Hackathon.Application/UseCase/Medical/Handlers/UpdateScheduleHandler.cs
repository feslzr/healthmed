using FIAP.PosTech.Hackathon.Application.Interfaces.Repository;
using FIAP.PosTech.Hackathon.Domain.Contracts;
using FIAP.PosTech.Hackathon.Domain.Entity;
using FIAP.PosTech.Hackathon.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace FIAP.PosTech.Hackathon.Application.UseCase.Medical.Handlers;

[ExcludeFromCodeCoverage]
public class UpdateScheduleUseCase : IRequest<Schedule>
{
    [BindProperty(Name = "Id")]
    [Required(ErrorMessage = "O campo 'Id' da agenda é obrigatório")]
    public required int ScheduleId { get; set; }

    [BindProperty(Name = "Horário")]
    [Required(ErrorMessage = "O campo 'Horário' é obrigatório")]
    public required DateTime Datetime { get; set; }
}

public class UpdateScheduleHandler(ILoggedUser loggedUser,
                                   IDoctorRepository doctorRepository,
                                   IServiceRepository serviceRepository,
                                   IScheduleRepository scheduleRepository) : IRequestHandler<UpdateScheduleUseCase, Schedule>
{
    public async Task<Schedule> Handle(UpdateScheduleUseCase request, CancellationToken cancellationToken)
    {
        var errorMessage = "Não é possível realizar essa solicitação";

        if (!loggedUser.IsDoctorAccess())
            throw new BusinessRuleException(errorMessage);

        if (request.Datetime < DateTime.Now)
            throw new BusinessRuleException("Não é possível criar uma agenda com data menor que o dia atual");

        var login = loggedUser.GetUser();

        var doctor = await doctorRepository.GetByDocumentNumberAsync(login) ??
                     throw new BusinessRuleException(errorMessage);

        var schedule = await scheduleRepository.GetByIdAsync(request.ScheduleId);

        var existingService = await serviceRepository.GetByIdAsync(schedule.ServiceId);

        if (existingService.DoctorId != doctor.Id)
            throw new BusinessRuleException(errorMessage);

        schedule.Datetime = request.Datetime;

        scheduleRepository.Update(schedule);
        await scheduleRepository.SaveChangesAsync();

        return schedule;
    }
}