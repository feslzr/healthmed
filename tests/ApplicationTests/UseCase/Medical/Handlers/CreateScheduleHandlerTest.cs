using AutoFixture;
using FIAP.PosTech.Hackathon.Application.Boundaries.Medical;
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
public class CreateScheduleHandlerTest
{
    private readonly Fixture _fixture;
    private readonly Mock<ILoggedUser> _loggedUserMock;
    private readonly Mock<IDoctorRepository> _doctorRepositoryMock;
    private readonly Mock<IServiceRepository> _serviceRepositoryMock;
    private readonly Mock<IScheduleRepository> _scheduleRepositoryMock;
    private readonly CreateScheduleHandler _handler;

    public CreateScheduleHandlerTest()
    {
        _fixture = new Fixture();
        _loggedUserMock = new Mock<ILoggedUser>();
        _doctorRepositoryMock = new Mock<IDoctorRepository>();
        _serviceRepositoryMock = new Mock<IServiceRepository>();
        _scheduleRepositoryMock = new Mock<IScheduleRepository>();

        _handler = new CreateScheduleHandler(
            _loggedUserMock.Object,
            _doctorRepositoryMock.Object,
            _serviceRepositoryMock.Object,
            _scheduleRepositoryMock.Object
        );
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenUserIsNotDoctor()
    {
        var request = _fixture.Create<CreateScheduleUseCase>();
        _loggedUserMock.Setup(x => x.IsDoctorAccess()).Returns(false);

        var ex = await Assert.ThrowsAsync<BusinessRuleException>(() =>
            _handler.Handle(request, CancellationToken.None));

        Assert.Equal("Não é possível realizar essa solicitação", ex.Message);
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenDatetimeIsInPast()
    {
        var request = _fixture.Build<CreateScheduleUseCase>()
                              .With(x => x.Datetime, DateTime.Now.AddDays(-1))
                              .Create();

        _loggedUserMock.Setup(x => x.IsDoctorAccess()).Returns(true);

        var ex = await Assert.ThrowsAsync<BusinessRuleException>(() =>
            _handler.Handle(request, CancellationToken.None));

        Assert.Equal("Não é possível criar uma agenda com data menor que o dia atual", ex.Message);
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenDoctorNotFound()
    {
        var request = _fixture.Build<CreateScheduleUseCase>()
                              .With(x => x.Datetime, DateTime.Now.AddDays(1))
                              .Create();

        _loggedUserMock.Setup(x => x.IsDoctorAccess()).Returns(true);
        _loggedUserMock.Setup(x => x.GetUser()).Returns("crm123");
        _doctorRepositoryMock.Setup(d => d.GetByDocumentNumberAsync("crm123"))
                             .ReturnsAsync((Doctor?)null);

        var ex = await Assert.ThrowsAsync<BusinessRuleException>(() =>
            _handler.Handle(request, CancellationToken.None));

        Assert.Equal("Não é possível realizar essa solicitação", ex.Message);
    }

    [Fact]
    public async Task Handle_ShouldCreateSchedule_WhenValid()
    {
        var request = _fixture.Build<CreateScheduleUseCase>()
                              .With(x => x.Specialty, MedicalSpecialtyEnum.Cardiologia)
                              .With(x => x.Datetime, DateTime.Now.AddHours(2))
                              .Create();

        _loggedUserMock.Setup(x => x.IsDoctorAccess()).Returns(true);
        _loggedUserMock.Setup(x => x.GetUser()).Returns("crm123");
        _doctorRepositoryMock.Setup(d => d.GetByDocumentNumberAsync("crm123"))
                             .ReturnsAsync(new Doctor(2, "crm123"));

        _serviceRepositoryMock.Setup(s => s.GetByDoctorIdAndSpecialtyAsync(It.IsAny<int>(), It.IsAny<int>()))
                              .ReturnsAsync(new Service(2, (int)request.Specialty, 10));

        var result = await _handler.Handle(request, CancellationToken.None);

        Assert.NotNull(result);
        Assert.IsType<ScheduleOutput>(result);
    }
}