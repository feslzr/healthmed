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
public class UpdateScheduleHandlerTest
{
    private readonly Fixture _fixture;
    private readonly Mock<ILoggedUser> _loggedUserMock;
    private readonly Mock<IDoctorRepository> _doctorRepositoryMock;
    private readonly Mock<IServiceRepository> _serviceRepositoryMock;
    private readonly Mock<IScheduleRepository> _scheduleRepositoryMock;
    private readonly UpdateScheduleHandler _handler;

    public UpdateScheduleHandlerTest()
    {
        _fixture = new Fixture();
        _loggedUserMock = new Mock<ILoggedUser>();
        _doctorRepositoryMock = new Mock<IDoctorRepository>();
        _serviceRepositoryMock = new Mock<IServiceRepository>();
        _scheduleRepositoryMock = new Mock<IScheduleRepository>();

        _handler = new UpdateScheduleHandler(
            _loggedUserMock.Object,
            _doctorRepositoryMock.Object,
            _serviceRepositoryMock.Object,
            _scheduleRepositoryMock.Object
        );
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenUserIsNotDoctor()
    {
        var request = _fixture.Create<UpdateScheduleUseCase>();
        _loggedUserMock.Setup(x => x.IsDoctorAccess()).Returns(false);

        var ex = await Assert.ThrowsAsync<BusinessRuleException>(() =>
            _handler.Handle(request, CancellationToken.None));

        Assert.Equal("Não é possível realizar essa solicitação", ex.Message);
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenDatetimeIsInPast()
    {
        var request = _fixture.Build<UpdateScheduleUseCase>()
                              .With(x => x.Datetime, DateTime.Now.AddHours(-2))
                              .Create();

        _loggedUserMock.Setup(x => x.IsDoctorAccess()).Returns(true);

        var ex = await Assert.ThrowsAsync<BusinessRuleException>(() =>
            _handler.Handle(request, CancellationToken.None));

        Assert.Equal("Não é possível criar uma agenda com data menor que o dia atual", ex.Message);
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenDoctorNotFound()
    {
        var request = _fixture.Build<UpdateScheduleUseCase>()
                              .With(x => x.Datetime, DateTime.Now.AddDays(1))
                              .Create();

        _loggedUserMock.Setup(x => x.IsDoctorAccess()).Returns(true);
        _loggedUserMock.Setup(x => x.GetUser()).Returns("crm999");

        _doctorRepositoryMock.Setup(d => d.GetByDocumentNumberAsync("crm999"))
                             .ReturnsAsync((Doctor?)null);

        var ex = await Assert.ThrowsAsync<BusinessRuleException>(() =>
            _handler.Handle(request, CancellationToken.None));

        Assert.Equal("Não é possível realizar essa solicitação", ex.Message);
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenScheduleNotBelongsToDoctor()
    {
        var request = _fixture.Build<UpdateScheduleUseCase>()
                              .With(x => x.ScheduleId, 10)
                              .With(x => x.Datetime, DateTime.Now.AddDays(1))
                              .Create();

        _loggedUserMock.Setup(x => x.IsDoctorAccess()).Returns(true);
        _loggedUserMock.Setup(x => x.GetUser()).Returns("crm999");

        _doctorRepositoryMock.Setup(d => d.GetByDocumentNumberAsync("crm999"))
                             .ReturnsAsync(new Doctor(1, "crm999"));

        _scheduleRepositoryMock.Setup(s => s.GetByIdAsync(10))
                               .ReturnsAsync(new Schedule(10, DateTime.Now));

        _serviceRepositoryMock.Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                              .ReturnsAsync(new Service(2, (int)MedicalSpecialtyEnum.Neurologia, 10));

        var ex = await Assert.ThrowsAsync<BusinessRuleException>(() =>
            _handler.Handle(request, CancellationToken.None));

        Assert.Equal("Não é possível realizar essa solicitação", ex.Message);
    }

    [Fact]
    public async Task Handle_ShouldUpdateSchedule_WhenValid()
    {
        var request = _fixture.Build<UpdateScheduleUseCase>()
                              .With(x => x.ScheduleId, 77)
                              .With(x => x.Datetime, DateTime.Now.AddDays(2))
                              .Create();

        _loggedUserMock.Setup(x => x.IsDoctorAccess()).Returns(true);
        _loggedUserMock.Setup(x => x.GetUser()).Returns("crm123");

        _doctorRepositoryMock.Setup(d => d.GetByDocumentNumberAsync("crm123"))
                             .ReturnsAsync(new Doctor(10, "crm999") { Id = 10 });

        _scheduleRepositoryMock.Setup(s => s.GetByIdAsync(77))
                               .ReturnsAsync(new Schedule(77, DateTime.Now));

        _serviceRepositoryMock.Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                              .ReturnsAsync(new Service(10, (int)MedicalSpecialtyEnum.Psiquiatria, 10));

        var result = await _handler.Handle(request, CancellationToken.None);

        Assert.NotNull(result);
        Assert.IsType<Schedule>(result);
    }
}