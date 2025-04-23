using Asp.Versioning;
using FIAP.PosTech.Hackathon.Application.Boundaries.Customer;
using FIAP.PosTech.Hackathon.Application.UseCase.Customer.Handlers;
using FIAP.PosTech.Hackathon.Domain.Entity;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FIAP.PosTech.Hackathon.API.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("v{version:apiVersion}/[controller]")]
public class CustomerController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Realiza pesquisa dos serviços médicos disponíveis
    /// </summary>
    /// <param name="useCase">Campos disponíveis para filtro</param>
    /// <returns>Retorna uma lista informando o médico, especialidade, agenda e preço</returns>
    [HttpGet("[action]")]
    [ProducesResponseType(typeof(List<MedicalServiceOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetMedicalServiceList([FromQuery] GetMedicalServiceListUseCase useCase)
    {
        var result = await mediator.Send(useCase);

        return Ok(result);
    }

    /// <summary>
    /// Realiza criação de consulta
    /// </summary>
    /// <param name="useCase">ID da agenda para marcação de consulta</param>
    /// <returns>Retorna as informações da consulta cadastrado</returns>
    [HttpPost("[action]")]
    [ProducesResponseType(typeof(List<AppointmentOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateAppointment([FromQuery] CreateAppointmentUseCase useCase)
    {
        var result = await mediator.Send(useCase);

        return Ok(result);
    }

    /// <summary>
    /// Realiza o cancelamento de uma consulta agendada
    /// </summary
    /// <param name="useCase">Campos necessários para cancelamento da consulta</param>
    /// <returns>Retorna as informações da consulta atualizada</returns>
    [HttpPut("[action]")]
    [ProducesResponseType(typeof(Appointment), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CancelAppointment([FromQuery] CancelAppointmentUseCase useCase)
    {
        var result = await mediator.Send(useCase);

        return Ok(result);
    }
}