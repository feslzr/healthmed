using FIAP.PosTech.Hackathon.Application.Interfaces;
using FIAP.PosTech.Hackathon.Domain.Enums;
using FIAP.PosTech.Hackathon.Domain.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FIAP.PosTech.Hackathon.Application.Services;

public class AccessTokenService(IOptions<JwtSettings> options) : IAccessTokenService
{
    private readonly JwtSettings _jwtSettings = options.Value;

    public string? CreateAccessToken(string login, AccountTypeEnum accountType)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        var key = Encoding.UTF8.GetBytes(_jwtSettings.Secret);

        var claims = new[] {
            new Claim(JwtRegisteredClaimNames.Name, login),
            new Claim("acc", accountType.ToString())
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                                                        SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}