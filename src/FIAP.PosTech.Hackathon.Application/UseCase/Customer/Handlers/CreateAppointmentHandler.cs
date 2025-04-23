using FIAP.PosTech.Hackathon.Application.Boundaries.Customer;
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

namespace FIAP.PosTech.Hackathon.Application.UseCase.Customer.Handlers;

[ExcludeFromCodeCoverage]
public class CreateAppointmentUseCase : IRequest<AppointmentOutput>
{
    [BindProperty(Name = "Id")]
    [Required(ErrorMessage = "O campo 'Id' da consulta é obrigatório")]
    public required int ScheduleId { get; set; }
}

public class CreateAppointmentHandler(ILoggedUser loggedUser,
                                      IAccountRepository accountRepository,
                                      IPatientRepository patientRepository,
                                      IDoctorRepository doctorRepository,
                                      IScheduleRepository scheduleRepository,
                                      IAppointmentRepository appointmentRepository,
                                      IServiceRepository serviceRepository) : IRequestHandler<CreateAppointmentUseCase, AppointmentOutput>
{
    public async Task<AppointmentOutput> Handle(CreateAppointmentUseCase request, CancellationToken cancellationToken)
    {
        var errorMessage = "Não é possível realizar essa solicitação";

        if (!loggedUser.IsPatientAccess())
            throw new BusinessRuleException(errorMessage);

        var login = loggedUser.GetUser();

        var patient = await patientRepository.GetByDocumentNumberAsync(login) ??
                      throw new BusinessRuleException(errorMessage);

        var schedule = await scheduleRepository.GetByIdAsync(request.ScheduleId);

        var statusIds = new[] { AppointmentStatusEnum.Confirmed, AppointmentStatusEnum.Pending }.Select(s => (int)s).ToArray();

        var existingAppointment = await appointmentRepository.GetByPatientIdAndScheduleIdAndStatusNullableAsync(patient.Id, schedule.Id, statusIds);

        if (existingAppointment != null)
            throw new BusinessRuleException("Já existe uma consulta para esse paciente e agenda, que está confirmada ou aguardando confirmação");

        var appointment = new Appointment(patient.Id, schedule.Id, (int)AppointmentStatusEnum.Pending);

        await appointmentRepository.AddAsync(appointment);
        await appointmentRepository.SaveChangesAsync();

        var service = await serviceRepository.GetByIdAsync(schedule.ServiceId);
        var doctor = await doctorRepository.GetByIdAsync(service.DoctorId);
        var doctorAccount = await accountRepository.GetByIdAsync(doctor.AccountId);
        var patientAccount = await accountRepository.GetByIdAsync(patient.AccountId);

        return new()
        {
            AppointmentId = appointment.Id,
            Datetime = schedule.Datetime,
            DoctorName = doctorAccount.Name,
            DoctorDocument = doctor.DocumentNumber,
            PatientName = patientAccount.Name,
            Price = service.Price,
            SpecialtyName = ((MedicalSpecialtyEnum)service.SpecialtyId).GetDescription(),
            CreatedAt = appointment.CreatedAt
        };
    }
}