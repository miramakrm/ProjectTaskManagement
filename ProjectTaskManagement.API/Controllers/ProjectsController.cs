using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectTaskManagement.Application.DTOs.Projects;
using ProjectTaskManagement.Domain.Entities;
using ProjectTaskManagement.Infrastructure.Persistence;
using System.Security.Claims;
using ProjectTaskManagement.Application.Common;

namespace ProjectTaskManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProjectsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProjectsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateProjectDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var exists = await _context.Projects
            .AnyAsync(p => p.Name.ToLower() == dto.Name.Trim().ToLower()
                      && p.UserId == userId);

        if (exists)
            return BadRequest(ApiResponse<string>.Fail("Project with this name already exists"));

        var project = new Project
        {
            Id = Guid.NewGuid(),
            Name = dto.Name.Trim(),
            Description = dto.Description,
            CreatedAt = DateTime.UtcNow,
            UserId = userId!
        };

        await _context.Projects.AddAsync(project);
        await _context.SaveChangesAsync();

        return Ok(ApiResponse<Project>.Ok(project, "Project created successfully"));
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var userId =
            User.FindFirstValue(ClaimTypes.NameIdentifier);

        var projects = await _context.Projects
            .Where(p => p.UserId == userId)
            .ToListAsync();

        return Ok(ApiResponse<List<Project>>.Ok(projects));
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var userId =
            User.FindFirstValue(ClaimTypes.NameIdentifier);

        var project = await _context.Projects
            .FirstOrDefaultAsync(p =>
                p.Id == id &&
                p.UserId == userId);
        if (project == null)
            return NotFound(ApiResponse<string>.Fail("Project not found"));

        return Ok(ApiResponse<Project>.Ok(project));
    }
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
    Guid id,
    UpdateProjectDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var project = await _context.Projects
            .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);

        if (project == null)
            return NotFound(ApiResponse<string>.Fail("Project not found"));

        // Check duplicate name (excluding current project)
        var exists = await _context.Projects
            .AnyAsync(p => p.Name == dto.Name && p.UserId == userId && p.Id != id);

        if (exists)
            return BadRequest(ApiResponse<string>.Fail("Project with this name already exists"));

        project.Name = dto.Name.Trim();
        project.Description = dto.Description;

        await _context.SaveChangesAsync();

        return Ok(ApiResponse<Project>.Ok(project, "Project updated successfully"));
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userId =
            User.FindFirstValue(ClaimTypes.NameIdentifier);

        var project = await _context.Projects
            .FirstOrDefaultAsync(p =>
                p.Id == id &&
                p.UserId == userId);

        if (project == null)
            return NotFound(ApiResponse<string>.Fail("Project not found"));

        _context.Projects.Remove(project);

        await _context.SaveChangesAsync();

        return Ok(ApiResponse<string>.Ok("Project deleted successfully"));
    }
}