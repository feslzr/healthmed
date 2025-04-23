using FIAP.PosTech.Hackathon.API.Controllers.V1;
using FIAP.PosTech.Hackathon.Application.Boundaries.Account;
using FIAP.PosTech.Hackathon.Application.UseCase.Account.Handlers;
using FIAP.PosTech.Hackathon.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Diagnostics.CodeAnalysis;

namespace APITests.Controllers.V1;

[ExcludeFromCodeCoverage]
public class AccountControllerTest
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly AccountController _controller;

    public AccountControllerTest()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new AccountController(_mediatorMock.Object);
    }

    [Fact]
    public async Task CreateDoctor_ShouldReturnOk()
    {
        var useCase = new CreateDoctorUseCase
        {
            Name = "Dr. Who",
            Password = "senha123",
            DocumentNumber = "123456"
        };

        var expected = new DoctorOutput { Name = "Dr. Who", DocumentNumber = "123456", CreatedAt = DateTime.Now };

        _mediatorMock.Setup(m => m.Send(useCase, default)).ReturnsAsync(expected);

        var result = await _controller.CreateDoctor(useCase);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        Assert.Equal(expected, okResult.Value);
    }

    [Fact]
    public async Task CreatePatient_ShouldReturnOk()
    {
        var useCase = new CreatePatientUseCase
        {
            Name = "Maria",
            Password = "123",
            DocumentNumber = "98765432100",
            Email = "maria@email.com"
        };

        var expected = new PatientOutput { Name = "Maria", DocumentNumber = "98765432100", Email = "maria@email.com", CreatedAt = DateTime.Now };

        _mediatorMock.Setup(m => m.Send(useCase, default)).ReturnsAsync(expected);

        var result = await _controller.CreatePatient(useCase);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        Assert.Equal(expected, okResult.Value);
    }

    [Fact]
    public async Task Login_ShouldReturnToken()
    {
        var useCase = new LoginUseCase
        {
            Login = "login",
            Password = "senha",
            AccountType = AccountTypeEnum.patient
        };

        _mediatorMock.Setup(m => m.Send(useCase, default)).ReturnsAsync("token123");

        var result = await _controller.Login(useCase);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        Assert.Equal("token123", okResult.Value);
    }
}