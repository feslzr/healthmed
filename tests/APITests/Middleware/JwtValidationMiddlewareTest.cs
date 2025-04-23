using FIAP.PosTech.Hackathon.API.Middleware;
using FIAP.PosTech.Hackathon.Domain.Contracts;
using FIAP.PosTech.Hackathon.Domain.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Moq;
using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace APITests.Middleware;

[ExcludeFromCodeCoverage]
public class JwtValidationMiddlewareTest
{
    private readonly Mock<RequestDelegate> _nextMock;
    private readonly Mock<ILoggedUser> _loggedUserMock;
    private readonly JwtSettings _jwtSettings;

    public JwtValidationMiddlewareTest()
    {
        _nextMock = new Mock<RequestDelegate>();
        _loggedUserMock = new Mock<ILoggedUser>();

        _jwtSettings = new JwtSettings
        {
            AuthorizationHeader = "Authorization",
            Issuer = "issuer",
            Audience = "audience",
            Secret = "this_is_a_very_secure_secret_key_123456!",
            ExpirationMinutes = 60
        };
    }

    [Fact]
    public async Task InvokeAsync_WithValidToken_ShouldSetLoggedUserAndContinue()
    {
        // Arrange
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_jwtSettings.Secret);

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, "login123"),
            new Claim("acc", "doctor"),
            new Claim("aud", _jwtSettings.Audience)
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(10),
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);

        var context = new DefaultHttpContext();
        context.Request.Headers[_jwtSettings.AuthorizationHeader] = $"Bearer {tokenString}";

        var middleware = new JwtValidationMiddleware(
            _nextMock.Object,
            Options.Create(_jwtSettings),
            tokenHandler
        );

        // Act
        await middleware.InvokeAsync(context, _loggedUserMock.Object);

        // Assert
        _loggedUserMock.Verify(x => x.SetUser("login123"), Times.Once);
        _loggedUserMock.Verify(x => x.SetDoctorAccess(true), Times.Once);
        _loggedUserMock.Verify(x => x.SetPatientAccess(false), Times.Once);
        _nextMock.Verify(x => x.Invoke(context), Times.Once);
    }

    [Fact]
    public async Task InvokeAsync_WithInvalidToken_ShouldReturn401()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.Headers[_jwtSettings.AuthorizationHeader] = "eyJhbGciOiJSUzI1NiIsImtpZCI6ImRKbks0dWVpQW5rVjBPRVZNQ3hEWWdRMkg0d3JxZk1NWVpCamlZdkt3Q1kiLCJ0eXAiOiJKV1QifQ.eyJ2ZXIiOiIxLjAiLCJpc3MiOiJodHRwczovL3RpY2tldGVzdGFibGlzaG1lbnRzbm9ucHJvZC5iMmNsb2dpbi5jb20vNGVkYzU1M2EtYWY5NS00ZjQxLWFkNTctZGI2OGNmMmFhMTViL3YyLjAvIiwic3ViIjoiYjFlOWRiMTAtZTkxMi00YTVhLWE4ZDYtNDk1ZjExZWI3MzM0IiwiYXVkIjoiNTE2NmRiZTgtNjFiNi00NmI1LWIxNDMtMTE2YWI0Nzc5YThhIiwiZXhwIjoxNzA1MzMwODQ4LCJhY3IiOiJiMmNfMWFfc2lnbnVwX3NpZ25pbiIsIm5vbmNlIjoiZGVmYXVsdE5vbmNlIiwiaWF0IjoxNzA1MzI5MDQ4LCJhdXRoX3RpbWUiOjE3MDUzMjkwNDgsIm5hbWUiOiIzNzUwMzkzMzAwMDEwMSIsInRpZCI6IjRlZGM1NTNhLWFmOTUtNGY0MS1hZDU3LWRiNjhjZjJhYTE1YiIsIm5iZiI6MTcwNTMyOTA0OH0.qNsyVjhUjEB5W0PZE8vtOsCl3bwHUOPb5jACiljcj3-lPBWBuACxJEihUW8j24HvC5CDoO4QsRxihkHnyGp-x7SG9yY-c19FTKXrMMmQcnXFfbd2kyG3b7KC7d3b1WVPs8u8RPr__5pxLavnGgfrFlmHyRDVX1GPQFxjJKUrzzaEhjvpKotv3hHdeuBHNGDpGJD8TBXMM4ggaDEsX2zlprLpeDJcZFPjOxB9GEMHV6AMN3TYePR6IHNuo9lyDSETuBs_ALjC2xmvrmkIgiufubFVJPw9bnCuQTY8xfwXqWD4Xzrdhsl0bUhrU7DG3wkDC95fxlhUja7kxkRm0GqsNQ";

        var middleware = new JwtValidationMiddleware(
            _nextMock.Object,
            Options.Create(_jwtSettings),
            new JwtSecurityTokenHandler()
        );

        // Act
        await middleware.InvokeAsync(context, _loggedUserMock.Object);

        // Assert
        Assert.Equal(StatusCodes.Status401Unauthorized, context.Response.StatusCode);
    }

    [Fact]
    public async Task InvokeAsync_WithoutAuthorizationHeader_ShouldContinueWithoutSettingUser()
    {
        // Arrange
        var context = new DefaultHttpContext();

        var middleware = new JwtValidationMiddleware(
            _nextMock.Object,
            Options.Create(_jwtSettings),
            new JwtSecurityTokenHandler()
        );

        // Act
        await middleware.InvokeAsync(context, _loggedUserMock.Object);

        // Assert
        _loggedUserMock.Verify(x => x.SetUser(It.IsAny<string>()), Times.Once);
        _nextMock.Verify(x => x.Invoke(context), Times.Once);
    }
}