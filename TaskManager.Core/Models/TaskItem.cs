namespace TaskManager.Core.Models;

public class TaskItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsFinished { get; set; }
    public TaskPriority Priority { get; set; } = TaskPriority.Medium;
    public DateTime? DueDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Notes written while working on this task.
    public List<Note> Notes { get; set; } = new();

    // Optional links to other modules.
    public int? CategoryId { get; set; }
    public Category? Category { get; set; }

    public int? LearningActivityId { get; set; }
    public LearningActivity? LearningActivity { get; set; }
}
