using FIAP.PosTech.Hackathon.Application.Interfaces.Repository;
using FIAP.PosTech.Hackathon.Domain.Contracts;
using FIAP.PosTech.Hackathon.Domain.Entity;
using FIAP.PosTech.Hackathon.Domain.Enums;
using FIAP.PosTech.Hackathon.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace FIAP.PosTech.Hackathon.Application.UseCase.Medical.Handlers;

[ExcludeFromCodeCoverage]
public class ConfirmAppointmentUseCase : IRequest<Appointment>
{
    [BindProperty(Name = "Id")]
    [Required(ErrorMessage = "O campo 'Id' da consulta é obrigatório")]
    public required int AppointmentId { get; set; }

    [BindProperty(Name = "Confirmação")]
    [Required(ErrorMessage = "O campo 'Confirmação' é obrigatório")]
    public required bool IsConfirmed { get; set; }
}

public class ConfirmAppointmentHandler(ILoggedUser loggedUser,
                                       IDoctorRepository doctorRepository,
                                       IScheduleRepository scheduleRepository,
                                       IServiceRepository serviceRepository,
                                       IAppointmentRepository appointmentRepository) : IRequestHandler<ConfirmAppointmentUseCase, Appointment>
{
    public async Task<Appointment> Handle(ConfirmAppointmentUseCase request, CancellationToken cancellationToken)
    {
        var errorMessage = "Não é possível realizar essa solicitação";

        if (!loggedUser.IsDoctorAccess())
            throw new BusinessRuleException(errorMessage);

        var login = loggedUser.GetUser();

        var doctor = await doctorRepository.GetByDocumentNumberAsync(login) ?? throw new BusinessRuleException(errorMessage);
        var appointment = await appointmentRepository.GetByIdAsync(request.AppointmentId);
        var schedule = await scheduleRepository.GetByIdAsync(appointment.ScheduleId);
        var service = await serviceRepository.GetByIdAsync(schedule.ServiceId);

        if (service.DoctorId != doctor.Id)
            throw new BusinessRuleException(errorMessage);

        appointment.StatusId = request.IsConfirmed ? (int)AppointmentStatusEnum.Confirmed :
                                                     (int)AppointmentStatusEnum.Refused;

        appointmentRepository.Update(appointment);
        await appointmentRepository.SaveChangesAsync();

        return appointment;
    }
}