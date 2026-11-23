namespace TaskManager.Core.Models;

public class Note
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public bool IsPinned { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // A note can belong to a task, a category and a study goal.
    public int? TaskId { get; set; }
    public TaskItem? Task { get; set; }

    public int? CategoryId { get; set; }
    public Category? Category { get; set; }

    public int? LearningActivityId { get; set; }
    public LearningActivity? LearningActivity { get; set; }
}
