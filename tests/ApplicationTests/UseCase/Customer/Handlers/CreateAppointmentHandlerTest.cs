using AutoFixture;
using FIAP.PosTech.Hackathon.Application.Boundaries.Customer;
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
public class CreateAppointmentHandlerTest
{
    private readonly Fixture _fixture;
    private readonly Mock<ILoggedUser> _loggedUserMock;
    private readonly Mock<IAccountRepository> _accountRepositoryMock;
    private readonly Mock<IPatientRepository> _patientRepositoryMock;
    private readonly Mock<IDoctorRepository> _doctorRepositoryMock;
    private readonly Mock<IScheduleRepository> _scheduleRepositoryMock;
    private readonly Mock<IAppointmentRepository> _appointmentRepositoryMock;
    private readonly Mock<IServiceRepository> _serviceRepositoryMock;
    private readonly CreateAppointmentHandler _handler;

    public CreateAppointmentHandlerTest()
    {
        _fixture = new Fixture();
        _loggedUserMock = new Mock<ILoggedUser>();
        _accountRepositoryMock = new Mock<IAccountRepository>();
        _patientRepositoryMock = new Mock<IPatientRepository>();
        _doctorRepositoryMock = new Mock<IDoctorRepository>();
        _scheduleRepositoryMock = new Mock<IScheduleRepository>();
        _appointmentRepositoryMock = new Mock<IAppointmentRepository>();
        _serviceRepositoryMock = new Mock<IServiceRepository>();

        _handler = new CreateAppointmentHandler(
            _loggedUserMock.Object,
            _accountRepositoryMock.Object,
            _patientRepositoryMock.Object,
            _doctorRepositoryMock.Object,
            _scheduleRepositoryMock.Object,
            _appointmentRepositoryMock.Object,
            _serviceRepositoryMock.Object
        );
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenUserIsNotPatient()
    {
        var request = _fixture.Create<CreateAppointmentUseCase>();
        _loggedUserMock.Setup(x => x.IsPatientAccess()).Returns(false);

        var ex = await Assert.ThrowsAsync<BusinessRuleException>(() =>
            _handler.Handle(request, CancellationToken.None));

        Assert.Equal("Não é possível realizar essa solicitação", ex.Message);
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenPatientNotFound()
    {
        var request = _fixture.Build<CreateAppointmentUseCase>()
                              .With(x => x.ScheduleId, 1)
                              .Create();

        _loggedUserMock.Setup(x => x.IsPatientAccess()).Returns(true);
        _loggedUserMock.Setup(x => x.GetUser()).Returns("cpf123");
        _patientRepositoryMock.Setup(p => p.GetByDocumentNumberAsync("cpf123")).ReturnsAsync((Patient?)null);

        var ex = await Assert.ThrowsAsync<BusinessRuleException>(() =>
            _handler.Handle(request, CancellationToken.None));

        Assert.Equal("Não é possível realizar essa solicitação", ex.Message);
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenAppointmentAlreadyExists()
    {
        var request = _fixture.Build<CreateAppointmentUseCase>()
                              .With(x => x.ScheduleId, 1)
                              .Create();

        _loggedUserMock.Setup(x => x.IsPatientAccess()).Returns(true);
        _loggedUserMock.Setup(x => x.GetUser()).Returns("cpf123");

        _patientRepositoryMock.Setup(p => p.GetByDocumentNumberAsync("cpf123"))
                              .ReturnsAsync(new Patient(10, "cpf123", "teste@email.com"));

        _scheduleRepositoryMock.Setup(s => s.GetByIdAsync(1))
                               .ReturnsAsync(new Schedule(1, DateTime.Now.AddDays(1)));

        _appointmentRepositoryMock.Setup(a => a.GetByPatientIdAndScheduleIdAndStatusNullableAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int[]>()))
                                  .ReturnsAsync(new Appointment(10, 1, (int)AppointmentStatusEnum.Confirmed));

        var ex = await Assert.ThrowsAsync<BusinessRuleException>(() =>
            _handler.Handle(request, CancellationToken.None));

        Assert.Equal("Já existe uma consulta para esse paciente e agenda, que está confirmada ou aguardando confirmação", ex.Message);
    }

    [Fact]
    public async Task Handle_ShouldCreateAppointment_WhenValid()
    {
        var request = _fixture.Build<CreateAppointmentUseCase>()
                              .With(x => x.ScheduleId, 1)
                              .Create();

        _loggedUserMock.Setup(x => x.IsPatientAccess()).Returns(true);
        _loggedUserMock.Setup(x => x.GetUser()).Returns("cpf123");

        _patientRepositoryMock.Setup(p => p.GetByDocumentNumberAsync("cpf123"))
                              .ReturnsAsync(new Patient(10, "cpf123", "teste@email.com"));
        _scheduleRepositoryMock.Setup(s => s.GetByIdAsync(1))
                               .ReturnsAsync(new Schedule(1, DateTime.Now.AddDays(1)));
        _serviceRepositoryMock.Setup(s => s.GetByIdAsync(1))
                              .ReturnsAsync(new Service(1, (int)MedicalSpecialtyEnum.Pediatria, 10));
        _doctorRepositoryMock.Setup(s => s.GetByIdAsync(1))
                             .ReturnsAsync(new Doctor(1, "crm123"));
        _accountRepositoryMock.SetupSequence(a => a.GetByIdAsync(It.IsAny<int>()))
                              .ReturnsAsync(new FIAP.PosTech.Hackathon.Domain.Entity.Account("Doutor", "senha"))
                              .ReturnsAsync(new FIAP.PosTech.Hackathon.Domain.Entity.Account("Paciente", "senha"));

        var result = await _handler.Handle(request, CancellationToken.None);

        Assert.NotNull(result);
        Assert.IsType<AppointmentOutput>(result);
    }
}