using AutoFixture;
using FIAP.PosTech.Hackathon.Application.Interfaces.Repository;
using FIAP.PosTech.Hackathon.Application.UseCase.Account.Handlers;
using FIAP.PosTech.Hackathon.Domain.Entity;
using FIAP.PosTech.Hackathon.Domain.Exceptions;
using Moq;
using System.Diagnostics.CodeAnalysis;

namespace ApplicationTests.UseCase.Account.Handlers;

[ExcludeFromCodeCoverage]
public class CreatePatientHandlerTest
{
    private readonly Fixture _fixture;
    private readonly Mock<IAccountRepository> _accountRepositoryMock;
    private readonly Mock<IPatientRepository> _patientRepositoryMock;
    private readonly CreatePatientHandler _handler;

    public CreatePatientHandlerTest()
    {
        _fixture = new Fixture();
        _accountRepositoryMock = new Mock<IAccountRepository>();
        _patientRepositoryMock = new Mock<IPatientRepository>();

        _handler = new CreatePatientHandler(_accountRepositoryMock.Object, _patientRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldThrowException_WhenPatientWithSameCpfOrEmailExists()
    {
        // Arrange
        var request = _fixture.Build<CreatePatientUseCase>()
                              .With(x => x.DocumentNumber, "12345678900")
                              .With(x => x.Email, "paciente@email.com")
                              .Create();

        var patient = new Patient(1, "12345678900", "paciente@email.com");

        _patientRepositoryMock.Setup(repo => repo.GetByDocumentNumberOrEmailAsync("12345678900", "paciente@email.com"))
                              .ReturnsAsync(patient);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<BusinessRuleException>(() =>
            _handler.Handle(request, CancellationToken.None));

        Assert.Equal("Já existe uma conta cadastrada com esse CPF ou e-mail", ex.Message);
    }

    [Fact]
    public async Task Handle_ShouldCreatePatientSuccessfully()
    {
        // Arrange
        var request = _fixture.Build<CreatePatientUseCase>()
                              .With(x => x.Name, "Maria Silva")
                              .With(x => x.Password, "abc123")
                              .With(x => x.DocumentNumber, "98765432100")
                              .With(x => x.Email, "maria@email.com")
                              .Create();

        _patientRepositoryMock.Setup(repo => repo.GetByDocumentNumberOrEmailAsync("98765432100", "maria@email.com"))
                              .ReturnsAsync((Patient?)null);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Maria Silva", result.Name);
        Assert.Equal("98765432100", result.DocumentNumber);
        Assert.Equal("maria@email.com", result.Email);
    }
}