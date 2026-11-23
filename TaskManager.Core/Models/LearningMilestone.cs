namespace TaskManager.Core.Models;

public class LearningMilestone
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
    public int LearningActivityId { get; set; }
    public LearningActivity? LearningActivity { get; set; }
}
