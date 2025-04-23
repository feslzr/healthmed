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
public class CreateServiceHandlerTest
{
    private readonly Fixture _fixture;
    private readonly Mock<ILoggedUser> _loggedUserMock;
    private readonly Mock<IDoctorRepository> _doctorRepositoryMock;
    private readonly Mock<IServiceRepository> _serviceRepositoryMock;
    private readonly CreateServiceHandler _handler;

    public CreateServiceHandlerTest()
    {
        _fixture = new Fixture();
        _loggedUserMock = new Mock<ILoggedUser>();
        _doctorRepositoryMock = new Mock<IDoctorRepository>();
        _serviceRepositoryMock = new Mock<IServiceRepository>();

        _handler = new CreateServiceHandler(
            _loggedUserMock.Object,
            _doctorRepositoryMock.Object,
            _serviceRepositoryMock.Object
        );
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenUserIsNotDoctor()
    {
        var request = _fixture.Create<CreateServiceUseCase>();
        _loggedUserMock.Setup(x => x.IsDoctorAccess()).Returns(false);

        var ex = await Assert.ThrowsAsync<BusinessRuleException>(() =>
            _handler.Handle(request, CancellationToken.None));

        Assert.Equal("Não é possível realizar essa solicitação", ex.Message);
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenDoctorNotFound()
    {
        var request = _fixture.Create<CreateServiceUseCase>();
        _loggedUserMock.Setup(x => x.IsDoctorAccess()).Returns(true);
        _loggedUserMock.Setup(x => x.GetUser()).Returns("crm123");

        _doctorRepositoryMock.Setup(d => d.GetByDocumentNumberAsync("crm123"))
                             .ReturnsAsync((Doctor?)null);

        var ex = await Assert.ThrowsAsync<BusinessRuleException>(() =>
            _handler.Handle(request, CancellationToken.None));

        Assert.Equal("Não é possível realizar essa solicitação", ex.Message);
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenServiceAlreadyExists()
    {
        var request = _fixture.Build<CreateServiceUseCase>()
                              .With(x => x.Specialty, MedicalSpecialtyEnum.Ortopedia)
                              .Create();

        _loggedUserMock.Setup(x => x.IsDoctorAccess()).Returns(true);
        _loggedUserMock.Setup(x => x.GetUser()).Returns("crm456");
        _doctorRepositoryMock.Setup(d => d.GetByDocumentNumberAsync("crm456"))
                             .ReturnsAsync(new Doctor(88, "crm456"));

        _serviceRepositoryMock.Setup(s => s.GetByDoctorIdAndSpecialtyNullableAsync(It.IsAny<int>(), It.IsAny<int>()))
                              .ReturnsAsync(new Service(88, (int)request.Specialty, 100));

        var ex = await Assert.ThrowsAsync<BusinessRuleException>(() =>
            _handler.Handle(request, CancellationToken.None));

        Assert.Equal("Já existe um serviço dessa especialidade para esse médico", ex.Message);
    }

    [Fact]
    public async Task Handle_ShouldCreateService_WhenValid()
    {
        var request = _fixture.Build<CreateServiceUseCase>()
                              .With(x => x.Specialty, MedicalSpecialtyEnum.Pediatria)
                              .With(x => x.Price, 200)
                              .Create();

        _loggedUserMock.Setup(x => x.IsDoctorAccess()).Returns(true);
        _loggedUserMock.Setup(x => x.GetUser()).Returns("crm456");

        _doctorRepositoryMock.Setup(d => d.GetByDocumentNumberAsync("crm456"))
                             .ReturnsAsync(new Doctor(22, "crm456"));

        _serviceRepositoryMock.Setup(s => s.GetByDoctorIdAndSpecialtyNullableAsync(22, (int)request.Specialty))
                              .ReturnsAsync((Service?)null);

        var result = await _handler.Handle(request, CancellationToken.None);

        Assert.NotNull(result);
        Assert.IsType<ServiceOutput>(result);
        Assert.Equal(200, result.Price);
        Assert.Equal(MedicalSpecialtyEnum.Pediatria.ToString(), result.SpecialtyName);
    }
}