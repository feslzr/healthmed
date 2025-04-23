using AutoFixture;
using FIAP.PosTech.Hackathon.Application.Interfaces.Repository;
using FIAP.PosTech.Hackathon.Application.UseCase.Medical.Handlers;
using FIAP.PosTech.Hackathon.Domain.Contracts;
using FIAP.PosTech.Hackathon.Domain.Entity;
using FIAP.PosTech.Hackathon.Domain.Enums;
using FIAP.PosTech.Hackathon.Domain.Exceptions;
using Moq;
using System.Diagnostics.CodeAnalysis;

namespace ApplicationTests.UseCase.Medical.Handlers;

[ExcludeFromCodeCoverage]
public class ConfirmAppointmentHandlerTest
{
    private readonly Fixture _fixture;
    private readonly Mock<ILoggedUser> _loggedUserMock;
    private readonly Mock<IDoctorRepository> _doctorRepositoryMock;
    private readonly Mock<IScheduleRepository> _scheduleRepositoryMock;
    private readonly Mock<IServiceRepository> _serviceRepositoryMock;
    private readonly Mock<IAppointmentRepository> _appointmentRepositoryMock;
    private readonly ConfirmAppointmentHandler _handler;

    public ConfirmAppointmentHandlerTest()
    {
        _fixture = new Fixture();
        _loggedUserMock = new Mock<ILoggedUser>();
        _doctorRepositoryMock = new Mock<IDoctorRepository>();
        _scheduleRepositoryMock = new Mock<IScheduleRepository>();
        _serviceRepositoryMock = new Mock<IServiceRepository>();
        _appointmentRepositoryMock = new Mock<IAppointmentRepository>();

        _handler = new ConfirmAppointmentHandler(
            _loggedUserMock.Object,
            _doctorRepositoryMock.Object,
            _scheduleRepositoryMock.Object,
            _serviceRepositoryMock.Object,
            _appointmentRepositoryMock.Object
        );
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenUserIsNotDoctor()
    {
        var request = _fixture.Create<ConfirmAppointmentUseCase>();
        _loggedUserMock.Setup(x => x.IsDoctorAccess()).Returns(false);

        var ex = await Assert.ThrowsAsync<BusinessRuleException>(() =>
            _handler.Handle(request, CancellationToken.None));

        Assert.Equal("Não é possível realizar essa solicitação", ex.Message);
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenDoctorNotFound()
    {
        var request = _fixture.Create<ConfirmAppointmentUseCase>();
        _loggedUserMock.Setup(x => x.IsDoctorAccess()).Returns(true);
        _loggedUserMock.Setup(x => x.GetUser()).Returns("crm123");

        _doctorRepositoryMock.Setup(d => d.GetByDocumentNumberAsync("crm123"))
                             .ReturnsAsync((Doctor?)null);

        var ex = await Assert.ThrowsAsync<BusinessRuleException>(() =>
            _handler.Handle(request, CancellationToken.None));

        Assert.Equal("Não é possível realizar essa solicitação", ex.Message);
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenDoctorIsDifferent()
    {
        var request = _fixture.Build<ConfirmAppointmentUseCase>()
                              .With(x => x.AppointmentId, 10)
                              .With(x => x.IsConfirmed, true)
                              .Create();

        var doctor = new Doctor(10, "crm123") { Id = 10 };
        var service = new Service(11, (int)MedicalSpecialtyEnum.Cardiologia, 100) { Id = 10 };
        var schedule = new Schedule(service.Id, DateTime.Now) { Id = 10 };
        var appointment = new Appointment(10, schedule.Id, (int)AppointmentStatusEnum.Confirmed) { Id = 10 };

        _loggedUserMock.Setup(x => x.IsDoctorAccess()).Returns(true);
        _loggedUserMock.Setup(x => x.GetUser()).Returns("crm123");

        _doctorRepositoryMock.Setup(d => d.GetByDocumentNumberAsync("crm123"))
                             .ReturnsAsync(doctor);

        _appointmentRepositoryMock.Setup(a => a.GetByIdAsync(10))
                                  .ReturnsAsync(appointment);

        _scheduleRepositoryMock.Setup(a => a.GetByIdAsync(10))
                               .ReturnsAsync(schedule);

        _serviceRepositoryMock.Setup(a => a.GetByIdAsync(10))
                              .ReturnsAsync(service);

        var ex = await Assert.ThrowsAsync<BusinessRuleException>(() =>
            _handler.Handle(request, CancellationToken.None));

        Assert.Equal("Não é possível realizar essa solicitação", ex.Message);
    }

    [Theory]
    [InlineData(true, (int)AppointmentStatusEnum.Confirmed)]
    [InlineData(false, (int)AppointmentStatusEnum.Refused)]
    public async Task Handle_ShouldUpdateAppointmentStatus_BasedOnConfirmation(bool confirm, int expected)
    {
        var request = _fixture.Build<ConfirmAppointmentUseCase>()
                              .With(x => x.AppointmentId, 10)
                              .With(x => x.IsConfirmed, confirm)
                              .Create();

        var doctor = new Doctor(10, "crm123") { Id = 10 };
        var service = new Service(doctor.Id, (int)MedicalSpecialtyEnum.Cardiologia, 100) { Id = 10 };
        var schedule = new Schedule(service.Id, DateTime.Now) { Id = 10 };
        var appointment = new Appointment(10, schedule.Id, expected) { Id = 10 };

        _loggedUserMock.Setup(x => x.IsDoctorAccess()).Returns(true);
        _loggedUserMock.Setup(x => x.GetUser()).Returns("crm123");

        _doctorRepositoryMock.Setup(d => d.GetByDocumentNumberAsync("crm123"))
                             .ReturnsAsync(doctor);

        _appointmentRepositoryMock.Setup(a => a.GetByIdAsync(10))
                                  .ReturnsAsync(appointment);

        _scheduleRepositoryMock.Setup(a => a.GetByIdAsync(10))
                               .ReturnsAsync(schedule);

        _serviceRepositoryMock.Setup(a => a.GetByIdAsync(10))
                              .ReturnsAsync(service);

        var result = await _handler.Handle(request, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(expected, result.StatusId);
    }
}