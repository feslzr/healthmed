using FIAP.PosTech.Hackathon.Application.Interfaces.Repository;
using FIAP.PosTech.Hackathon.Domain.Contracts;
using FIAP.PosTech.Hackathon.Domain.Entity;
using FIAP.PosTech.Hackathon.Domain.Enums;
using FIAP.PosTech.Hackathon.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace FIAP.PosTech.Hackathon.Application.UseCase.Customer.Handlers;

[ExcludeFromCodeCoverage]
public class CancelAppointmentUseCase : IRequest<Appointment>
{
    [BindProperty(Name = "Id")]
    [Required(ErrorMessage = "O campo 'Id' da consulta é obrigatório")]
    public required int AppointmentId { get; set; }

    [BindProperty(Name = "Justificativa")]
    [Required(ErrorMessage = "O campo 'Justificativa' é obrigatório")]
    public required string Justification { get; set; }
}

public class CancelAppointmentHandler(ILoggedUser loggedUser,
                                      IPatientRepository patientRepository,
                                      IAppointmentRepository appointmentRepository) : IRequestHandler<CancelAppointmentUseCase, Appointment>
{
    public async Task<Appointment> Handle(CancelAppointmentUseCase request, CancellationToken cancellationToken)
    {
        var errorMessage = "Não é possível realizar essa solicitação";

        if (!loggedUser.IsPatientAccess())
            throw new BusinessRuleException(errorMessage);

        var login = loggedUser.GetUser();

        var patient = await patientRepository.GetByDocumentNumberAsync(login) ?? throw new BusinessRuleException(errorMessage);
        var appointment = await appointmentRepository.GetByIdAsync(request.AppointmentId);

        if (appointment.PatientId != patient.Id)
            throw new BusinessRuleException(errorMessage);

        appointment.StatusId = (int)AppointmentStatusEnum.Canceled;
        appointment.Justification = request.Justification;

        appointmentRepository.Update(appointment);
        await appointmentRepository.SaveChangesAsync();

        return appointment;
    }
}