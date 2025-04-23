using FIAP.PosTech.Hackathon.Domain.Contracts;
using FIAP.PosTech.Hackathon.Domain.Enums;
using FIAP.PosTech.Hackathon.Domain.Settings;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FIAP.PosTech.Hackathon.API.Middleware;

public class JwtValidationMiddleware(RequestDelegate next,
                                     IOptions<JwtSettings> options,
                                     JwtSecurityTokenHandler? jwtHandler = null)
{
    private JwtSettings JwtSettings { get; set; } = options.Value;

    private bool _isDoctorAccess = false;
    private bool _isPatientAccess = false;

    private readonly RequestDelegate _next = next;
    private readonly JwtSecurityTokenHandler _tokenHandler = jwtHandler ?? new JwtSecurityTokenHandler();

    public async Task InvokeAsync(HttpContext context, ILoggedUser loggedUser)
    {
        var user = await GetValidationTokenAsync(context);

        loggedUser.SetUser(user);
        loggedUser.SetDoctorAccess(_isDoctorAccess);
        loggedUser.SetPatientAccess(_isPatientAccess);

        await _next.Invoke(context);
    }

    #region Auxiliar Methods

    async Task<string> GetValidationTokenAsync(HttpContext context)
    {
        var user = string.Empty;

        if (context.Request.Headers.TryGetValue(JwtSettings.AuthorizationHeader, out StringValues value))
        {
            var token = ParseJWT(value);
            var audience = GetAudienceClaim(token);
            var accountType = GetAccClaim(token);

            Enum.TryParse(accountType, true, out AccountTypeEnum accountEnum);

            var validationParameters = GetValidationParametersAsync(audience);

            try
            {
                var tokenValidation = _tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

                if (validatedToken is JwtSecurityToken)
                {
                    user = GetLogin(tokenValidation);
                    _isDoctorAccess = accountEnum == AccountTypeEnum.doctor;
                    _isPatientAccess = accountEnum == AccountTypeEnum.patient;
                }
            } catch (Exception)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Operação não permitida");
            }
        }

        return user;
    }

    public TokenValidationParameters GetValidationParametersAsync(string audience)
    {
        var key = Encoding.UTF8.GetBytes(JwtSettings.Secret);

        return new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),

            ValidateIssuer = true,
            ValidIssuer = JwtSettings.Issuer,

            ValidateAudience = true,
            ValidAudience = audience,

            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    }

    static string? ParseJWT(string? authorization)
    {
        var jwtParsed = authorization;

        if (!string.IsNullOrEmpty(authorization))
        {
            var bearerWord = authorization[..6].ToLower();

            if (bearerWord.Equals("bearer"))
                jwtParsed = authorization[7..];
        }

        return jwtParsed;
    }

    static string GetAudienceClaim(StringValues? authorization)
    {
        var token = new JwtSecurityTokenHandler().ReadJwtToken(authorization);
        return token.Claims.First(f => f.Type.Equals("aud")).Value;
    }

    static string? GetAccClaim(StringValues? authorization)
    {
        var token = new JwtSecurityTokenHandler().ReadJwtToken(authorization);
        return token.Claims.FirstOrDefault(f => f.Type.Equals("acc"))?.Value;
    }

    static string GetLogin(ClaimsPrincipal principal)
    {
        var claims = principal.Claims;

        var name = claims.FirstOrDefault(f => f.Type == ClaimTypes.Name) ??
                   claims.FirstOrDefault(f => f.Type.Equals("name"));

        return name == null ? string.Empty : name.Value;
    }

    #endregion
}