using System.ComponentModel.DataAnnotations;

namespace ProjectTaskManagement.Application.DTOs.Projects;

public class CreateProjectDto
{
    [Required(ErrorMessage = "Name is required")]
    [MinLength(3, ErrorMessage = "Name must be at least 3 characters")]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }
}