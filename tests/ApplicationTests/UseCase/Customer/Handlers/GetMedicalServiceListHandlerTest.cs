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
public class GetMedicalServiceListHandlerTest
{
    private readonly Fixture _fixture;
    private readonly Mock<ILoggedUser> _loggedUserMock;
    private readonly Mock<IPatientRepository> _patientRepositoryMock;
    private readonly Mock<IDoctorRepository> _doctorRepositoryMock;
    private readonly Mock<IScheduleRepository> _scheduleRepositoryMock;
    private readonly GetMedicalServiceListHandler _handler;

    public GetMedicalServiceListHandlerTest()
    {
        _fixture = new Fixture();
        _loggedUserMock = new Mock<ILoggedUser>();
        _patientRepositoryMock = new Mock<IPatientRepository>();
        _doctorRepositoryMock = new Mock<IDoctorRepository>();
        _scheduleRepositoryMock = new Mock<IScheduleRepository>();

        _handler = new GetMedicalServiceListHandler(
            _loggedUserMock.Object,
            _patientRepositoryMock.Object,
            _doctorRepositoryMock.Object,
            _scheduleRepositoryMock.Object
        );
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenUserIsNotPatientAndIsNotDoctor()
    {
        var request = _fixture.Create<GetMedicalServiceListUseCase>();

        _loggedUserMock.Setup(x => x.IsPatientAccess()).Returns(false);
        _loggedUserMock.Setup(x => x.IsDoctorAccess()).Returns(false);

        var ex = await Assert.ThrowsAsync<BusinessRuleException>(() =>
            _handler.Handle(request, CancellationToken.None));

        Assert.Equal("Não é possível realizar essa solicitação", ex.Message);
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenPatientNotFound()
    {
        var request = _fixture.Create<GetMedicalServiceListUseCase>();
        _loggedUserMock.Setup(x => x.IsPatientAccess()).Returns(true);
        _loggedUserMock.Setup(x => x.GetUser()).Returns("cpf123");

        _patientRepositoryMock.Setup(p => p.GetByDocumentNumberAsync("cpf123"))
                              .ReturnsAsync((Patient?)null);

        var ex = await Assert.ThrowsAsync<BusinessRuleException>(() =>
            _handler.Handle(request, CancellationToken.None));

        Assert.Equal("Não é possível realizar essa solicitação", ex.Message);
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenDoctorNotFound()
    {
        var request = _fixture.Create<GetMedicalServiceListUseCase>();
        _loggedUserMock.Setup(x => x.IsDoctorAccess()).Returns(true);
        _loggedUserMock.Setup(x => x.GetUser()).Returns("crm123");

        _doctorRepositoryMock.Setup(p => p.GetByDocumentNumberAsync("crm123"))
                             .ReturnsAsync((Doctor?)null);

        var ex = await Assert.ThrowsAsync<BusinessRuleException>(() =>
            _handler.Handle(request, CancellationToken.None));

        Assert.Equal("Não é possível realizar essa solicitação", ex.Message);
    }

    [Fact]
    public async Task Handle_ShouldReturnMedicalServices_WhenValid()
    {
        var request = _fixture.Build<GetMedicalServiceListUseCase>()
                              .With(x => x.DoctorName, "Dr. João")
                              .With(x => x.Specialty, MedicalSpecialtyEnum.Cardiologia)
                              .With(x => x.Status, AppointmentStatusEnum.Pending)
                              .Create();

        _loggedUserMock.Setup(x => x.IsPatientAccess()).Returns(true);
        _loggedUserMock.Setup(x => x.GetUser()).Returns("cpf123");

        _patientRepositoryMock.Setup(p => p.GetByDocumentNumberAsync("cpf123"))
                              .ReturnsAsync(new Patient(1, "cpf123", "teste@email.com"));

        var services = _fixture.CreateMany<MedicalServiceOutput>(3).ToList();
        _scheduleRepositoryMock.Setup(s => s.GetMedicalServicesAsync(
            request.DoctorName,
            (int?)request.Specialty,
            request.StartDate,
            request.EndDate,
            request.Price,
            (int?)request.Status
        )).ReturnsAsync(services);

        var result = await _handler.Handle(request, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
    }
}