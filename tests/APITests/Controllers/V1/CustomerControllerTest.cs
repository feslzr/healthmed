using AutoFixture;
using FIAP.PosTech.Hackathon.API.Controllers.V1;
using FIAP.PosTech.Hackathon.Application.Boundaries.Customer;
using FIAP.PosTech.Hackathon.Application.UseCase.Customer.Handlers;
using FIAP.PosTech.Hackathon.Domain.Entity;
using FIAP.PosTech.Hackathon.Domain.Enums;
using FIAP.PosTech.Hackathon.Domain.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Diagnostics.CodeAnalysis;

namespace APITests.Controllers.V1;

[ExcludeFromCodeCoverage]
public class CustomerControllerTest
{
    private readonly Fixture _fixture;

    private readonly Mock<IMediator> _mediatorMock;
    private readonly CustomerController _controller;

    public CustomerControllerTest()
    {
        _fixture = new Fixture();
        _mediatorMock = new Mock<IMediator>();
        _controller = new CustomerController(_mediatorMock.Object);
    }

    [Fact]
    public async Task GetMedicalServiceList_ShouldReturnOk()
    {
        var useCase = new GetMedicalServiceListUseCase { DoctorName = "Dr. João" };
        var expected = new List<MedicalServiceOutput> {
            new() {
                DoctorName = "Dr. João",
                SpecialtyDescription = MedicalSpecialtyEnum.Psicologia.GetDescription(),
                AppointmentStatusDescription = AppointmentStatusEnum.Pending.GetDescription()
            }
        };

        _mediatorMock.Setup(m => m.Send(useCase, default)).ReturnsAsync(expected);

        var result = await _controller.GetMedicalServiceList(useCase);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(expected, ok.Value);
    }

    [Fact]
    public async Task CreateAppointment_ShouldReturnOk()
    {
        var useCase = new CreateAppointmentUseCase { ScheduleId = 1 };
        var expected = _fixture.Create<AppointmentOutput>();

        _mediatorMock.Setup(m => m.Send(useCase, default)).ReturnsAsync(expected);

        var result = await _controller.CreateAppointment(useCase);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(expected, ok.Value);
    }

    [Fact]
    public async Task CancelAppointment_ShouldReturnOk()
    {
        var useCase = new CancelAppointmentUseCase
        {
            AppointmentId = 2,
            Justification = "não poderei comparecer"
        };

        var expected = new Appointment(2, 1, (int)AppointmentStatusEnum.Canceled);

        _mediatorMock.Setup(m => m.Send(useCase, default)).ReturnsAsync(expected);

        var result = await _controller.CancelAppointment(useCase);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(expected, ok.Value);
    }
}