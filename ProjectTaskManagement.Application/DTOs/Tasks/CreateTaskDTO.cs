using ProjectTaskManagement.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace ProjectTaskManagement.Application.DTOs.Tasks;

public class CreateTaskDto
{
    [Required(ErrorMessage = "Title is required")]
    [MinLength(3, ErrorMessage = "Title must be at least 3 characters")]
    public string Title { get; set; } = string.Empty;


    public string? Description { get; set; }

    public DateTime DueDate { get; set; }

    public TaskPriority Priority { get; set; }

    [Required(ErrorMessage = "ProjectId is required")]
    public Guid ProjectId { get; set; }
}