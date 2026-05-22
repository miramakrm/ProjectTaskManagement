using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectTaskManagement.Application.Common;
using ProjectTaskManagement.Application.DTOs.Tasks;
using ProjectTaskManagement.Domain.Entities;
using ProjectTaskManagement.Infrastructure.Persistence;
using System.Security.Claims;
using TaskItemStatus = ProjectTaskManagement.Domain.Enums.TaskItemStatus;

namespace ProjectTaskManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TasksController : ControllerBase
{
    private readonly AppDbContext _context;

    public TasksController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateTaskDto dto)
    {
        var userId =
            User.FindFirstValue(ClaimTypes.NameIdentifier);

        var project = await _context.Projects
            .FirstOrDefaultAsync(p =>
                p.Id == dto.ProjectId &&
                p.UserId == userId);

        if (project == null)
            return NotFound(ApiResponse<string>.Fail("Project not found"));

        var taskItem = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = dto.Title,
            Description = dto.Description,
            DueDate = dto.DueDate,
            Priority = dto.Priority,
            Status = TaskItemStatus.Pending,
            ProjectId = dto.ProjectId
        };

        await _context.Tasks.AddAsync(taskItem);

        await _context.SaveChangesAsync();

        return Ok(ApiResponse<TaskItem>.Ok(taskItem, "Task created successfully"));
    }

    [HttpGet("project/{projectId}")]
    public async Task<IActionResult> GetByProject(Guid projectId)
    {
        var userId =
            User.FindFirstValue(ClaimTypes.NameIdentifier);

        var project = await _context.Projects
            .FirstOrDefaultAsync(p =>
                p.Id == projectId &&
                p.UserId == userId);

        if (project == null)
        {
            return NotFound(ApiResponse<string>.Fail("Project not found"));
        }

        var tasks = await _context.Tasks
            .Where(t => t.ProjectId == projectId)
            .ToListAsync();

        return Ok(ApiResponse<List<TaskItem>>.Ok(tasks));
    }

    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus(
        Guid id,
        UpdateTaskStatusDto dto)
    {
        var task = await _context.Tasks
            .FirstOrDefaultAsync(t => t.Id == id);

        if (task == null)
        {
            return NotFound(ApiResponse<string>.Fail("Task not found"));
        }

        task.Status = dto.Status;

        await _context.SaveChangesAsync();

        return Ok(ApiResponse<TaskItem>.Ok(task, "Task status updated successfully"));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var task = await _context.Tasks
            .FirstOrDefaultAsync(t => t.Id == id);

        if (task == null)
        {
            return NotFound(ApiResponse<string>.Fail("Task not found"));
        }

        _context.Tasks.Remove(task);

        await _context.SaveChangesAsync();

        return Ok(ApiResponse<string>.Ok("Task deleted successfully"));
    }
}