using FIAP.PosTech.Hackathon.Application.Boundaries.Customer;
using FIAP.PosTech.Hackathon.Application.Interfaces.Repository;
using FIAP.PosTech.Hackathon.Domain.Contracts;
using FIAP.PosTech.Hackathon.Domain.Enums;
using FIAP.PosTech.Hackathon.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;

namespace FIAP.PosTech.Hackathon.Application.UseCase.Customer.Handlers;

[ExcludeFromCodeCoverage]
public class GetMedicalServiceListUseCase : IRequest<List<MedicalServiceOutput>>
{
    [BindProperty(Name = "Médico")]
    public string? DoctorName { get; set; }

    [BindProperty(Name = "Especialidade")]
    public MedicalSpecialtyEnum? Specialty { get; set; }

    [BindProperty(Name = "Status")]
    public AppointmentStatusEnum? Status { get; set; }

    [BindProperty(Name = "Início")]
    public DateTime? StartDate { get; set; } = DateTime.Now;

    [BindProperty(Name = "Fim")]
    public DateTime? EndDate { get; set; } = DateTime.Now.AddDays(30);

    [BindProperty(Name = "Valor")]
    public decimal? Price { get; set; }
}

public class GetMedicalServiceListHandler(ILoggedUser loggedUser,
                                          IPatientRepository patientRepository,
                                          IDoctorRepository doctorRepository,
                                          IScheduleRepository scheduleRepository) : IRequestHandler<GetMedicalServiceListUseCase, List<MedicalServiceOutput>>
{
    public async Task<List<MedicalServiceOutput>> Handle(GetMedicalServiceListUseCase request, CancellationToken cancellationToken)
    {
        var errorMessage = "Não é possível realizar essa solicitação";

        if (!loggedUser.IsPatientAccess() && !loggedUser.IsDoctorAccess())
            throw new BusinessRuleException(errorMessage);

        var login = loggedUser.GetUser();

        var patient = await patientRepository.GetByDocumentNumberAsync(login);
        var doctor = await doctorRepository.GetByDocumentNumberAsync(login);

        if (patient == null && doctor == null)
            throw new BusinessRuleException(errorMessage);

        int? specialtyId = request.Specialty.HasValue ? (int)request.Specialty.Value : null;
        int? statusId = request.Status.HasValue ? (int)request.Status.Value : null;

        return await scheduleRepository.GetMedicalServicesAsync(request.DoctorName, specialtyId, request.StartDate,
                                                                request.EndDate, request.Price, statusId);
    }
}