using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Moq;
using ProjectTaskManagement.API.Controllers;
using ProjectTaskManagement.Application.Common;
using ProjectTaskManagement.Application.DTOs.Auth;
using ProjectTaskManagement.Application.Interfaces;
using ProjectTaskManagement.Infrastructure.Identity;

namespace ProjectTaskManagement.Tests;

public class AuthControllerTests
{
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
    private readonly Mock<IJwtService> _jwtServiceMock;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _userManagerMock = new Mock<UserManager<ApplicationUser>>(
            Mock.Of<IUserStore<ApplicationUser>>(),
            null, null, null, null, null, null, null, null);

        _jwtServiceMock = new Mock<IJwtService>();

        _controller = new AuthController(_userManagerMock.Object, _jwtServiceMock.Object);
    }

    [Fact]
    public async Task Register_WhenUserAlreadyExists_ReturnsBadRequest()
    {
        // Arrange
        var dto = new RegisterDto { Email = "test@test.com", Password = "Pass123!", UserName = "test" };
        _userManagerMock.Setup(x => x.FindByEmailAsync(dto.Email))
            .ReturnsAsync(new ApplicationUser());

        // Act
        var result = await _controller.Register(dto);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Register_WhenValidData_ReturnsOk()
    {
        // Arrange
        var dto = new RegisterDto { Email = "test@test.com", Password = "Pass123!", UserName = "test" };
        _userManagerMock.Setup(x => x.FindByEmailAsync(dto.Email))
            .ReturnsAsync((ApplicationUser)null!);
        _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), dto.Password))
            .ReturnsAsync(IdentityResult.Success);
        _jwtServiceMock.Setup(x => x.GenerateToken(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns("fake-token");

        // Act
        var result = await _controller.Register(dto);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task Login_WhenInvalidCredentials_ReturnsUnauthorized()
    {
        // Arrange
        var dto = new LoginDto { Email = "test@test.com", Password = "wrongpass" };
        _userManagerMock.Setup(x => x.FindByEmailAsync(dto.Email))
            .ReturnsAsync((ApplicationUser)null!);

        // Act
        var result = await _controller.Login(dto);

        // Assert
        result.Should().BeOfType<UnauthorizedObjectResult>();
    }

    [Fact]
    public async Task Login_WhenValidCredentials_ReturnsOkWithToken()
    {
        // Arrange
        var dto = new LoginDto { Email = "test@test.com", Password = "Pass123!" };
        var user = new ApplicationUser { Id = "123", Email = dto.Email, UserName = "test" };
        _userManagerMock.Setup(x => x.FindByEmailAsync(dto.Email)).ReturnsAsync(user);
        _userManagerMock.Setup(x => x.CheckPasswordAsync(user, dto.Password)).ReturnsAsync(true);
        _jwtServiceMock.Setup(x => x.GenerateToken(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns("fake-token");

        // Act
        var result = await _controller.Login(dto);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse<AuthResponseDto>>().Subject;
        response.Success.Should().BeTrue();
        response.Data!.Token.Should().Be("fake-token");
    }
}