using AutoFixture;
using FIAP.PosTech.Hackathon.Application.Interfaces;
using FIAP.PosTech.Hackathon.Application.Interfaces.Repository;
using FIAP.PosTech.Hackathon.Application.UseCase.Account.Handlers;
using FIAP.PosTech.Hackathon.Domain.Enums;
using FIAP.PosTech.Hackathon.Domain.Exceptions;
using Moq;
using System.Diagnostics.CodeAnalysis;

namespace ApplicationTests.UseCase.Account.Handlers;

[ExcludeFromCodeCoverage]
public class LoginHandlerTest
{
    private const string _password = "teste123";
    private const string _token = "a1b2c3d4e5";

    private readonly Fixture _fixture;

    private readonly Mock<IAccountRepository> _accountRepositoryMock;
    private readonly Mock<IDoctorRepository> _doctorRepositoryMock;
    private readonly Mock<IPatientRepository> _patientRepositoryMock;
    private readonly Mock<IAccessTokenService> _accessTokenServiceMock;

    private readonly LoginHandler _handler;

    public LoginHandlerTest()
    {
        _fixture = new Fixture();
        _accountRepositoryMock = new Mock<IAccountRepository>();
        _doctorRepositoryMock = new Mock<IDoctorRepository>();
        _patientRepositoryMock = new Mock<IPatientRepository>();
        _accessTokenServiceMock = new Mock<IAccessTokenService>();

        _handler = new LoginHandler(
            _accountRepositoryMock.Object,
            _doctorRepositoryMock.Object,
            _patientRepositoryMock.Object,
            _accessTokenServiceMock.Object
        );
    }

    [Fact]
    public async Task Handle_ShouldLoginDoctorSuccessfully()
    {
        // Arrange
        var account = new FIAP.PosTech.Hackathon.Domain.Entity.Account("Name Surname", "uYrsFLiGRiQlrLg+L3b6WjiLU/m0oStUkBkNkMTPZdM=");

        var useCase = _fixture.Build<LoginUseCase>()
                              .With(x => x.AccountType, AccountTypeEnum.doctor)
                              .With(x => x.Password, _password)
                              .Create();

        _doctorRepositoryMock.Setup(repo => repo.GetAccountIdByDocumentNumberAsync(useCase.Login))
                             .ReturnsAsync(1);
        _accountRepositoryMock.Setup(repo => repo.GetAccountByIdAsync(1))
                              .ReturnsAsync(account);
        _accessTokenServiceMock.Setup(service => service.CreateAccessToken(useCase.Login, useCase.AccountType))
                               .Returns(_token);

        // Act
        var result = await _handler.Handle(useCase, CancellationToken.None);

        // Assert
        Assert.Equal(_token, result);
    }

    [Fact]
    public async Task Handle_ShouldLoginPatientSuccessfully()
    {
        // Arrange
        var account = new FIAP.PosTech.Hackathon.Domain.Entity.Account("Name Surname", "uYrsFLiGRiQlrLg+L3b6WjiLU/m0oStUkBkNkMTPZdM=");

        var useCase = _fixture.Build<LoginUseCase>()
                              .With(x => x.AccountType, AccountTypeEnum.patient)
                              .With(x => x.Password, _password)
                              .Create();

        _patientRepositoryMock.Setup(repo => repo.GetAccountIdByLoginAsync(useCase.Login))
                              .ReturnsAsync(2);
        _accountRepositoryMock.Setup(repo => repo.GetAccountByIdAsync(2))
                              .ReturnsAsync(account);
        _accessTokenServiceMock.Setup(service => service.CreateAccessToken(useCase.Login, useCase.AccountType))
                               .Returns(_token);

        // Act
        var result = await _handler.Handle(useCase, CancellationToken.None);

        // Assert
        Assert.Equal(_token, result);
    }

    [Fact]
    public async Task Handle_InvalidPassword_ShouldThrowException()
    {
        // Arrange
        var account = new FIAP.PosTech.Hackathon.Domain.Entity.Account("Name Surname", "uYrsFLiGRiQlrLg+L3b6WjiLU/m0oStUkBkNkMTPZdM");

        var useCase = _fixture.Build<LoginUseCase>()
                              .With(x => x.AccountType, AccountTypeEnum.patient)
                              .With(x => x.Password, _password)
                              .Create();

        _patientRepositoryMock.Setup(repo => repo.GetAccountIdByLoginAsync(useCase.Login))
                              .ReturnsAsync(2);
        _accountRepositoryMock.Setup(repo => repo.GetAccountByIdAsync(2))
                              .ReturnsAsync(account);
        _accessTokenServiceMock.Setup(service => service.CreateAccessToken(useCase.Login, useCase.AccountType))
                               .Returns(_token);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<BusinessRuleException>(() => _handler.Handle(useCase, CancellationToken.None));
        Assert.Equal("Usuário ou senha inválido, tente novamente", ex.Message);
    }

    [Fact]
    public async Task Handle_InvalidAccessToken_ShouldThrowException()
    {
        // Arrange
        var account = new FIAP.PosTech.Hackathon.Domain.Entity.Account("Name Surname", "uYrsFLiGRiQlrLg+L3b6WjiLU/m0oStUkBkNkMTPZdM=");

        var useCase = _fixture.Build<LoginUseCase>()
                              .With(x => x.AccountType, AccountTypeEnum.patient)
                              .With(x => x.Password, _password)
                              .Create();

        _patientRepositoryMock.Setup(repo => repo.GetAccountIdByLoginAsync(useCase.Login))
                              .ReturnsAsync(2);
        _accountRepositoryMock.Setup(repo => repo.GetAccountByIdAsync(2))
                              .ReturnsAsync(account);
        _accessTokenServiceMock.Setup(service => service.CreateAccessToken(useCase.Login, useCase.AccountType))
                               .Returns(string.Empty);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<BusinessRuleException>(() => _handler.Handle(useCase, CancellationToken.None));
        Assert.Contains("Não foi possível gerar um token de acesso", ex.Message);
    }

    [Fact]
    public async Task Handle_InvalidLogin_ShouldThrowException()
    {
        // Arrange
        var useCase = _fixture.Build<LoginUseCase>()
                              .With(x => x.AccountType, AccountTypeEnum.doctor)
                              .Create();

        _doctorRepositoryMock.Setup(repo => repo.GetAccountIdByDocumentNumberAsync(useCase.Login))
                             .ReturnsAsync(0);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<BusinessRuleException>(() => _handler.Handle(useCase, CancellationToken.None));
        Assert.Equal("Usuário ou senha inválido, tente novamente", ex.Message);
    }
}