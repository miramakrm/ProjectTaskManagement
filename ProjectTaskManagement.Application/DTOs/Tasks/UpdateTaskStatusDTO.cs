using ProjectTaskManagement.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using TaskItemStatus = ProjectTaskManagement.Domain.Enums.TaskItemStatus;
namespace ProjectTaskManagement.Application.DTOs.Tasks;

public class UpdateTaskStatusDto
{
    [Required(ErrorMessage = "Status is required")]
    public TaskItemStatus Status { get; set; }
}