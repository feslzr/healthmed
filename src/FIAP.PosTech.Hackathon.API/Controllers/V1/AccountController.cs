using Asp.Versioning;
using FIAP.PosTech.Hackathon.Application.Boundaries.Account;
using FIAP.PosTech.Hackathon.Application.UseCase.Account.Handlers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FIAP.PosTech.Hackathon.API.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("v{version:apiVersion}/[controller]")]
public class AccountController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Realiza criação de uma nova conta de médico
    /// </summary>
    /// <param name="useCase">Campos necessários para criação de uma nova conta (médico)</param>
    /// <returns>Retorna as informações do médico cadastrado</returns>
    [HttpPost("[action]")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(DoctorOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateDoctor([FromBody] CreateDoctorUseCase useCase)
    {
        var result = await mediator.Send(useCase);

        return Ok(result);
    }

    /// <summary>
    /// Realiza criação de uma nova conta de paciente
    /// </summary>
    /// <param name="useCase">Campos necessários para criação de uma nova conta (paciente)</param>
    /// <returns>Retorna as informações do paciente cadastrado</returns>
    [HttpPost("[action]")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(PatientOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreatePatient([FromBody] CreatePatientUseCase useCase)
    {
        var result = await mediator.Send(useCase);

        return Ok(result);
    }

    /// <summary>
    /// Realiza login e obtém um token de acesso JWT
    /// </summary>
    /// <param name="useCase">Campos necessários para realizar login</param>
    /// <returns>Retorna o token de acesso JWT para autenticação</returns>
    [HttpGet("[action]")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Login([FromQuery] LoginUseCase useCase)
    {
        var result = await mediator.Send(useCase);

        return Ok(result);
    }
}