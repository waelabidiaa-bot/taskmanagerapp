using TaskManager.Core.Interfaces;
using TaskManager.Core.Models;

namespace TaskManager.Core.Services;

public class LearningService
{
    private readonly ILearningRepository _repository;

    public LearningService(ILearningRepository repository)
    {
        _repository = repository;
    }

    public Task<List<LearningActivity>> GetAllAsync()
    {
        return _repository.GetAllAsync();
    }

    public async Task<LearningActivity> CreateAsync(
        string title,
        string description,
        LearningStatus status = LearningStatus.Planned)
    {
        // Progress starts at 0; it is only changed when milestones are completed.
        var activity = new LearningActivity
        {
            Title = title,
            Description = description,
            Progress = 0,
            Status = status
        };

        Validate(activity);
        await _repository.AddAsync(activity);

        return activity;
    }

    public async Task UpdateAsync(LearningActivity activity)
    {
        RecalculateProgress(activity);
        Validate(activity);
        await _repository.UpdateAsync(activity);
    }

    public async Task AddMilestoneAsync(int activityId, string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Milestone title is required.");

        var activity = await GetActivityOrThrowAsync(activityId);
        activity.Milestones.Add(new LearningMilestone { Title = title.Trim() });
        await SaveMilestonesAsync(activity);
    }

    public async Task ToggleMilestoneAsync(int activityId, int milestoneId)
    {
        var activity = await GetActivityOrThrowAsync(activityId);
        var milestone = activity.Milestones.FirstOrDefault(m => m.Id == milestoneId)
            ?? throw new InvalidOperationException("Milestone not found.");

        milestone.IsCompleted = !milestone.IsCompleted;
        await SaveMilestonesAsync(activity);
    }

    public async Task DeleteMilestoneAsync(int activityId, int milestoneId)
    {
        var activity = await GetActivityOrThrowAsync(activityId);
        var milestone = activity.Milestones.FirstOrDefault(m => m.Id == milestoneId)
            ?? throw new InvalidOperationException("Milestone not found.");

        activity.Milestones.Remove(milestone);
        RecalculateProgress(activity);
        Validate(activity);

        // Save the updated progress first, then remove the milestone itself.
        await _repository.UpdateAsync(activity);
        await _repository.DeleteMilestoneAsync(milestoneId);
    }

    public Task DeleteAsync(int id)
    {
        return _repository.DeleteAsync(id);
    }

    private async Task<LearningActivity> GetActivityOrThrowAsync(int activityId)
    {
        return await _repository.GetByIdAsync(activityId)
            ?? throw new InvalidOperationException("Study goal not found.");
    }

    private async Task SaveMilestonesAsync(LearningActivity activity)
    {
        RecalculateProgress(activity);
        Validate(activity);
        await _repository.UpdateAsync(activity);
    }

    // The single source of truth for progress: completed milestones divided by all milestones.
    private static void RecalculateProgress(LearningActivity activity)
    {
        var total = activity.Milestones.Count;
        activity.Progress = total == 0
            ? 0
            : (int)Math.Round(activity.Milestones.Count(m => m.IsCompleted) * 100.0 / total);

        if (total == 0)
            return;

        if (activity.Progress == 100)
            activity.Status = LearningStatus.Completed;
        else if (activity.Status == LearningStatus.Completed)
            activity.Status = LearningStatus.InProgress;
    }

    private static void Validate(LearningActivity activity)
    {
        if (string.IsNullOrWhiteSpace(activity.Title))
            throw new ArgumentException("Title is required.");

        if (activity.Progress is < 0 or > 100)
            throw new ArgumentException("Progress must be between 0 and 100.");

        activity.Title = activity.Title.Trim();
        activity.Description ??= string.Empty;
    }
}
