namespace ProjectTaskManagement.Domain.Entities;

public class Project
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public string UserId { get; set; } = string.Empty;

    public ICollection<TaskItem> Tasks { get; set; }
        = new List<TaskItem>();
}