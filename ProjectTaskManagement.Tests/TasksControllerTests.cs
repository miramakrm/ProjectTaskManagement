using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectTaskManagement.API.Controllers;
using ProjectTaskManagement.Application.Common;
using ProjectTaskManagement.Application.DTOs.Tasks;
using ProjectTaskManagement.Domain.Entities;
using ProjectTaskManagement.Domain.Enums;
using ProjectTaskManagement.Infrastructure.Persistence;
using System.Security.Claims;

namespace ProjectTaskManagement.Tests;

public class TasksControllerTests
{
    private readonly AppDbContext _context;
    private readonly TasksController _controller;
    private const string UserId = "test-user-id";
    private readonly Guid _projectId = Guid.NewGuid();

    public TasksControllerTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _controller = new TasksController(_context);

        var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, UserId) };
        var identity = new ClaimsIdentity(claims);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
        };

        
        _context.Projects.Add(new Project
        {
            Id = _projectId,
            Name = "Test Project",
            UserId = UserId,
            CreatedAt = DateTime.UtcNow
        });
        _context.SaveChanges();
    }

    [Fact]
    public async Task Create_WhenValidProject_ReturnsOk()
    {
        // Arrange
        var dto = new CreateTaskDto
        {
            Title = "Test Task",
            ProjectId = _projectId,
            Priority = TaskPriority.Medium
        };

        // Act
        var result = await _controller.Create(dto);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse<TaskItem>>().Subject;
        response.Success.Should().BeTrue();
        response.Data!.Title.Should().Be("Test Task");
    }

    [Fact]
    public async Task Create_WhenProjectNotFound_ReturnsNotFound()
    {
        // Arrange
        var dto = new CreateTaskDto { Title = "Test Task", ProjectId = Guid.NewGuid() };

        // Act
        var result = await _controller.Create(dto);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task UpdateStatus_WhenTaskExists_ReturnsOk()
    {
        // Arrange
        var task = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = "Test Task",
            Status = TaskItemStatus.Pending,
            ProjectId = _projectId
        };
        await _context.Tasks.AddAsync(task);
        await _context.SaveChangesAsync();

        var dto = new UpdateTaskStatusDto { Status = TaskItemStatus.InProgress };

   
        var result = await _controller.UpdateStatus(task.Id, dto);

     
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse<TaskItem>>().Subject;
        response.Data!.Status.Should().Be(TaskItemStatus.InProgress);
    }

    [Fact]
    public async Task Delete_WhenTaskNotFound_ReturnsNotFound()
    {
        
        var result = await _controller.Delete(Guid.NewGuid());

       
        result.Should().BeOfType<NotFoundObjectResult>();
    }
}