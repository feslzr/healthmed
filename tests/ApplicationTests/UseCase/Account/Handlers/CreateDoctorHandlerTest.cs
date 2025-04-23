using AutoFixture;
using FIAP.PosTech.Hackathon.Application.Interfaces.Repository;
using FIAP.PosTech.Hackathon.Application.UseCase.Account.Handlers;
using FIAP.PosTech.Hackathon.Domain.Entity;
using FIAP.PosTech.Hackathon.Domain.Exceptions;
using Moq;
using System.Diagnostics.CodeAnalysis;

namespace ApplicationTests.UseCase.Account.Handlers;

[ExcludeFromCodeCoverage]
public class CreateDoctorHandlerTest
{
    private readonly Fixture _fixture;
    private readonly Mock<IAccountRepository> _accountRepositoryMock;
    private readonly Mock<IDoctorRepository> _doctorRepositoryMock;
    private readonly CreateDoctorHandler _handler;

    public CreateDoctorHandlerTest()
    {
        _fixture = new Fixture();
        _accountRepositoryMock = new Mock<IAccountRepository>();
        _doctorRepositoryMock = new Mock<IDoctorRepository>();

        _handler = new CreateDoctorHandler(_accountRepositoryMock.Object, _doctorRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldThrowException_WhenDoctorWithSameCrmExists()
    {
        // Arrange
        var request = _fixture.Build<CreateDoctorUseCase>()
                              .With(x => x.DocumentNumber, "123456")
                              .Create();

        var doctor = _fixture.Build<Doctor>().Create();

        _doctorRepositoryMock.Setup(repo => repo.GetByDocumentNumberAsync("123456"))
                             .ReturnsAsync(doctor);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<BusinessRuleException>(() =>
            _handler.Handle(request, CancellationToken.None));

        Assert.Equal("Já existe uma conta cadastrada com esse CRM", ex.Message);
    }

    [Fact]
    public async Task Handle_ShouldCreateDoctorSuccessfully()
    {
        // Arrange
        var request = _fixture.Build<CreateDoctorUseCase>()
                              .With(x => x.Name, "Dr. House")
                              .With(x => x.Password, "senha123")
                              .With(x => x.DocumentNumber, "987654")
                              .Create();

        _doctorRepositoryMock.Setup(repo => repo.GetByDocumentNumberAsync("987654"))
                             .ReturnsAsync((Doctor?)null);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Dr. House", result.Name);
        Assert.Equal("987654", result.DocumentNumber);
    }
}