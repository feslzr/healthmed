using AutoFixture;
using FIAP.PosTech.Hackathon.API.Controllers.V1;
using FIAP.PosTech.Hackathon.Application.Boundaries.Medical;
using FIAP.PosTech.Hackathon.Application.UseCase.Medical.Handlers;
using FIAP.PosTech.Hackathon.Domain.Entity;
using FIAP.PosTech.Hackathon.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Diagnostics.CodeAnalysis;

namespace APITests.Controllers.V1;

[ExcludeFromCodeCoverage]
public class MedicalControllerTest
{
    private readonly Fixture _fixture;

    private readonly Mock<IMediator> _mediatorMock;
    private readonly MedicalController _controller;

    public MedicalControllerTest()
    {
        _fixture = new Fixture();
        _mediatorMock = new Mock<IMediator>();
        _controller = new MedicalController(_mediatorMock.Object);
    }

    [Fact]
    public async Task CreateService_ShouldReturnOk()
    {
        var useCase = new CreateServiceUseCase
        {
            Specialty = MedicalSpecialtyEnum.Pediatria,
            Price = 150
        };

        var expected = _fixture.Create<ServiceOutput>();

        _mediatorMock.Setup(m => m.Send(useCase, default)).ReturnsAsync(expected);

        var result = await _controller.CreateService(useCase);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(expected, ok.Value);
    }

    [Fact]
    public async Task CreateSchedule_ShouldReturnOk()
    {
        var useCase = new CreateScheduleUseCase
        {
            Specialty = MedicalSpecialtyEnum.Cardiologia,
            Datetime = DateTime.Now.AddDays(1)
        };

        var expected = _fixture.Create<ScheduleOutput>();

        _mediatorMock.Setup(m => m.Send(useCase, default)).ReturnsAsync(expected);

        var result = await _controller.CreateSchedule(useCase);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(expected, ok.Value);
    }

    [Fact]
    public async Task UpdateSchedule_ShouldReturnOk()
    {
        var useCase = new UpdateScheduleUseCase
        {
            ScheduleId = 5,
            Datetime = DateTime.Now.AddHours(4)
        };

        var expected = _fixture.Create<Schedule>();

        _mediatorMock.Setup(m => m.Send(useCase, default)).ReturnsAsync(expected);

        var result = await _controller.UpdateSchedule(useCase);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(expected, ok.Value);
    }

    [Fact]
    public async Task ConfirmAppointment_ShouldReturnOk()
    {
        var useCase = new ConfirmAppointmentUseCase
        {
            AppointmentId = 2,
            IsConfirmed = true
        };

        var expected = _fixture.Create<Appointment>();

        _mediatorMock.Setup(m => m.Send(useCase, default)).ReturnsAsync(expected);

        var result = await _controller.ConfirmAppointment(useCase);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(expected, ok.Value);
    }
}