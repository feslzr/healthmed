/// <param name="useCase">Campos necessários para edição de agenda</param>
using Asp.Versioning;
using FIAP.PosTech.Hackathon.Application.Boundaries.Medical;
using FIAP.PosTech.Hackathon.Application.UseCase.Medical.Handlers;
using FIAP.PosTech.Hackathon.Domain.Entity;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FIAP.PosTech.Hackathon.API.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("v{version:apiVersion}/[controller]")]
public class MedicalController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Realiza criação de um novo serviço médico
    /// </summary>
    /// <param name="useCase">Campos necessários para criação de serviço</param>
    /// <returns>Retorna as informações do serviço cadastrado</returns>
    [HttpPost("[action]")]
    [ProducesResponseType(typeof(ServiceOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateService([FromQuery] CreateServiceUseCase useCase)
    {
        var result = await mediator.Send(useCase);

        return Ok(result);
    }

    /// <summary>
    /// Realiza criação de uma nova agenda disponível
    /// </summary>
    /// <param name="useCase">Campos necessários para criação de agenda</param>
    /// <returns>Retorna as informações da agenda cadastrada</returns>
    [HttpPost("[action]")]
    [ProducesResponseType(typeof(ScheduleOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateSchedule([FromQuery] CreateScheduleUseCase useCase)
    {
        var result = await mediator.Send(useCase);

        return Ok(result);
    }

    /// <summary>
    /// Realiza edição de uma agenda cadastrada
    /// </summary
    /// <param name="useCase">Campos necessários para edição de agenda</param>
    /// <returns>Retorna as informações da agenda atualizada</returns>
    [HttpPut("[action]")]
    [ProducesResponseType(typeof(Schedule), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateSchedule([FromQuery] UpdateScheduleUseCase useCase)
    {
        var result = await mediator.Send(useCase);

        return Ok(result);
    }

    /// <summary>
    /// Realiza a confirmação ou não de uma consulta agendada
    /// </summary
    /// <param name="useCase">Campos necessários para confirmação da consulta</param>
    /// <returns>Retorna as informações da consulta atualizada</returns>
    [HttpPut("[action]")]
    [ProducesResponseType(typeof(Appointment), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ConfirmAppointment([FromQuery] ConfirmAppointmentUseCase useCase)
    {
        var result = await mediator.Send(useCase);

        return Ok(result);
    }
}