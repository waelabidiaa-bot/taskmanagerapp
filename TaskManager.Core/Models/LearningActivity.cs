namespace TaskManager.Core.Models;

public class LearningActivity
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Progress { get; set; }
    public LearningStatus Status { get; set; } = LearningStatus.Planned;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<TaskItem> Tasks { get; set; } = new();
    public List<Note> Notes { get; set; } = new();
    public List<LearningMilestone> Milestones { get; set; } = new();

    // Derived values used by the UI; they are not stored in the database.
    public int CompletedTasks => Tasks.Count(t => t.IsFinished);
    public int CompletedMilestones => Milestones.Count(m => m.IsCompleted);
}
