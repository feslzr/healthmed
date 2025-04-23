using FIAP.PosTech.Hackathon.Application.Interfaces;
using FIAP.PosTech.Hackathon.Application.Interfaces.Repository;
using FIAP.PosTech.Hackathon.Domain.Enums;
using FIAP.PosTech.Hackathon.Domain.Exceptions;
using FIAP.PosTech.Hackathon.Domain.Helpers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace FIAP.PosTech.Hackathon.Application.UseCase.Account.Handlers;

[ExcludeFromCodeCoverage]
public class LoginUseCase : IRequest<string>
{
    [BindProperty(Name = "Login")]
    [Required(ErrorMessage = "O campo 'Login' é obrigatório")]
    public required string Login { get; set; }

    [BindProperty(Name = "Senha")]
    [Required(ErrorMessage = "O campo 'Senha' é obrigatório")]
    public required string Password { get; set; }

    [BindProperty(Name = "Tipo")]
    [Required(ErrorMessage = "É necessário informar o tipo de conta")]
    public required AccountTypeEnum AccountType { get; set; }
}

public class LoginHandler(IAccountRepository accountRepository,
                          IDoctorRepository doctorRepository,
                          IPatientRepository patientRepository,
                          IAccessTokenService accessToken) : IRequestHandler<LoginUseCase, string>
{
    public async Task<string> Handle(LoginUseCase request, CancellationToken cancellationToken)
    {
        var errorMessage = "Usuário ou senha inválido, tente novamente";
        var accountId = 0;

        if (request.AccountType == AccountTypeEnum.doctor)
            accountId = await doctorRepository.GetAccountIdByDocumentNumberAsync(request.Login);
        else if (request.AccountType == AccountTypeEnum.patient)
            accountId = await patientRepository.GetAccountIdByLoginAsync(request.Login);

        if (accountId == 0)
            throw new BusinessRuleException(errorMessage);

        var account = await accountRepository.GetAccountByIdAsync(accountId);

        if (!PasswordHelper.VerifyPassword(request.Password, account.Password))
            throw new BusinessRuleException(errorMessage);

        var tokenJwt = accessToken.CreateAccessToken(request.Login, request.AccountType);

        if (string.IsNullOrEmpty(tokenJwt))
            throw new BusinessRuleException("Não foi possível gerar um token de acesso para o usuário solicitado");

        return tokenJwt;
    }
}