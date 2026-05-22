using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using ProjectTaskManagement.API.Controllers;
using ProjectTaskManagement.Application.Common;
using ProjectTaskManagement.Application.DTOs.Projects;
using ProjectTaskManagement.Domain.Entities;
using ProjectTaskManagement.Infrastructure.Persistence;
using System.Security.Claims;

namespace ProjectTaskManagement.Tests;

public class ProjectsControllerTests
{
    private readonly AppDbContext _context;
    private readonly ProjectsController _controller;
    private const string UserId = "test-user-id";

    public ProjectsControllerTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _controller = new ProjectsController(_context);

        // Mock the User
        var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, UserId) };
        var identity = new ClaimsIdentity(claims);
        var claimsPrincipal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };
    }

    [Fact]
    public async Task Create_WhenValidData_ReturnsOk()
    {
        // Arrange
        var dto = new CreateProjectDto { Name = "Test Project", Description = "Test" };

        // Act
        var result = await _controller.Create(dto);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse<Project>>().Subject;
        response.Success.Should().BeTrue();
        response.Data!.Name.Should().Be("Test Project");
    }

    [Fact]
    public async Task Create_WhenDuplicateName_ReturnsBadRequest()
    {
        // Arrange
        var dto = new CreateProjectDto { Name = "Test Project" };
        await _controller.Create(dto);

        // Act
        var result = await _controller.Create(dto);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task GetAll_ReturnsOnlyUserProjects()
    {
        // Arrange
        await _context.Projects.AddAsync(new Project
        {
            Id = Guid.NewGuid(),
            Name = "My Project",
            UserId = UserId,
            CreatedAt = DateTime.UtcNow
        });
        await _context.Projects.AddAsync(new Project
        {
            Id = Guid.NewGuid(),
            Name = "Other Project",
            UserId = "other-user",
            CreatedAt = DateTime.UtcNow
        });
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse<List<Project>>>().Subject;
        response.Data.Should().HaveCount(1);
        response.Data![0].Name.Should().Be("My Project");
    }

    [Fact]
    public async Task GetById_WhenNotFound_ReturnsNotFound()
    {
        // Act
        var result = await _controller.GetById(Guid.NewGuid());

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }
}