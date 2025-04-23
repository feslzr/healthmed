using FIAP.PosTech.Hackathon.Domain.Enums;

namespace FIAP.PosTech.Hackathon.Application.Interfaces;

public interface IAccessTokenService
{
    string? CreateAccessToken(string login, AccountTypeEnum accountType);
}