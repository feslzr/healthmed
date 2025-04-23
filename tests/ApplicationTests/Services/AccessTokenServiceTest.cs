using FIAP.PosTech.Hackathon.Application.Services;
using FIAP.PosTech.Hackathon.Domain.Enums;
using FIAP.PosTech.Hackathon.Domain.Settings;
using Microsoft.Extensions.Options;
using Moq;
using System.Diagnostics.CodeAnalysis;

namespace ApplicationTests.Services;

[ExcludeFromCodeCoverage]
public class AccessTokenServiceTest
{
    private readonly AccessTokenService _service;

    private readonly Mock<IOptions<JwtSettings>> _jwtOptionsMock;

    public AccessTokenServiceTest()
    {
        _jwtOptionsMock = new Mock<IOptions<JwtSettings>>();

        var settings = new JwtSettings
        {
            AuthorizationHeader = "Authorization",
            Secret = "supersecretkeythatshouldbeatleast32characterslong",
            Issuer = "UnitTestIssuer",
            Audience = "UnitTestAudience",
            ExpirationMinutes = 60
        };

        _jwtOptionsMock.Setup(o => o.Value).Returns(settings);

        _service = new AccessTokenService(_jwtOptionsMock.Object);
    }

    [Theory]
    [InlineData("user1", AccountTypeEnum.patient)]
    [InlineData("medic123", AccountTypeEnum.doctor)]
    public void CreateAccessToken_ShouldGenerateValidToken(string login, AccountTypeEnum accountType)
    {
        // Act
        var token = _service.CreateAccessToken(login, accountType);

        // Assert
        Assert.False(string.IsNullOrEmpty(token));
        Assert.IsType<string>(token);
    }

    [Fact]
    public void CreateAccessToken_WithNullLogin_ShouldStillGenerateToken()
    {
        // Act
        var token = _service.CreateAccessToken(string.Empty, AccountTypeEnum.patient);

        // Assert
        Assert.False(string.IsNullOrEmpty(token));
    }
}