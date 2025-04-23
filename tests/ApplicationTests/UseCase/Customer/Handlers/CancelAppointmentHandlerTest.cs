using AutoFixture;
using FIAP.PosTech.Hackathon.Application.Interfaces.Repository;
using FIAP.PosTech.Hackathon.Application.UseCase.Customer.Handlers;
using FIAP.PosTech.Hackathon.Domain.Contracts;
using FIAP.PosTech.Hackathon.Domain.Entity;
using FIAP.PosTech.Hackathon.Domain.Enums;
using FIAP.PosTech.Hackathon.Domain.Exceptions;
using Moq;
using System.Diagnostics.CodeAnalysis;

namespace ApplicationTests.UseCase.Customer.Handlers;

[ExcludeFromCodeCoverage]
public class CancelAppointmentHandlerTest
{
    private readonly Fixture _fixture;
    private readonly Mock<ILoggedUser> _loggedUserMock;
    private readonly Mock<IPatientRepository> _patientRepositoryMock;
    private readonly Mock<IAppointmentRepository> _appointmentRepositoryMock;
    private readonly CancelAppointmentHandler _handler;

    public CancelAppointmentHandlerTest()
    {
        _fixture = new Fixture();
        _loggedUserMock = new Mock<ILoggedUser>();
        _patientRepositoryMock = new Mock<IPatientRepository>();
        _appointmentRepositoryMock = new Mock<IAppointmentRepository>();

        _handler = new CancelAppointmentHandler(
            _loggedUserMock.Object,
            _patientRepositoryMock.Object,
            _appointmentRepositoryMock.Object
        );
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenUserIsNotPatient()
    {
        var request = _fixture.Create<CancelAppointmentUseCase>();
        _loggedUserMock.Setup(x => x.IsPatientAccess()).Returns(false);

        var ex = await Assert.ThrowsAsync<BusinessRuleException>(() =>
            _handler.Handle(request, CancellationToken.None));

        Assert.Equal("Não é possível realizar essa solicitação", ex.Message);
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenPatientNotFound()
    {
        var request = _fixture.Create<CancelAppointmentUseCase>();
        _loggedUserMock.Setup(x => x.IsPatientAccess()).Returns(true);
        _loggedUserMock.Setup(x => x.GetUser()).Returns("cpf123");

        _patientRepositoryMock.Setup(p => p.GetByDocumentNumberAsync("cpf123"))
                              .ReturnsAsync((Patient?)null);

        var ex = await Assert.ThrowsAsync<BusinessRuleException>(() =>
            _handler.Handle(request, CancellationToken.None));

        Assert.Equal("Não é possível realizar essa solicitação", ex.Message);
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenPatientIsDifferent()
    {
        var request = _fixture.Build<CancelAppointmentUseCase>()
                              .With(x => x.AppointmentId, 2)
                              .With(x => x.Justification, "Indisponível")
                              .Create();

        var patient = new Patient(20, "cpf123", "teste@email.com") { Id = 20 };

        _loggedUserMock.Setup(x => x.IsPatientAccess()).Returns(true);
        _loggedUserMock.Setup(x => x.GetUser()).Returns("cpf123");

        _patientRepositoryMock.Setup(p => p.GetByDocumentNumberAsync("cpf123"))
                              .ReturnsAsync(patient);

        _appointmentRepositoryMock.Setup(a => a.GetByIdAsync(2))
                                  .ReturnsAsync(new Appointment(10, 1, (int)AppointmentStatusEnum.Canceled));

        var ex = await Assert.ThrowsAsync<BusinessRuleException>(() =>
            _handler.Handle(request, CancellationToken.None));

        Assert.Equal("Não é possível realizar essa solicitação", ex.Message);
    }

    [Fact]
    public async Task Handle_ShouldCancelAppointment_WhenValid()
    {
        var request = _fixture.Build<CancelAppointmentUseCase>()
                              .With(x => x.AppointmentId, 2)
                              .With(x => x.Justification, "Indisponível")
                              .Create();

        var patient = new Patient(20, "cpf123", "teste@email.com") { Id = 20 };

        _loggedUserMock.Setup(x => x.IsPatientAccess()).Returns(true);
        _loggedUserMock.Setup(x => x.GetUser()).Returns("cpf123");

        _patientRepositoryMock.Setup(p => p.GetByDocumentNumberAsync("cpf123"))
                              .ReturnsAsync(patient);

        _appointmentRepositoryMock.Setup(a => a.GetByIdAsync(2))
                                  .ReturnsAsync(new Appointment(20, 1, (int)AppointmentStatusEnum.Canceled));

        var result = await _handler.Handle(request, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal((int)AppointmentStatusEnum.Canceled, result.StatusId);
        Assert.Equal("Indisponível", result.Justification);
    }
}