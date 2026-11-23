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
        int progress,
        LearningStatus status)
    {
        var activity = new LearningActivity
        {
            Title = title,
            Description = description,
            Progress = progress,
            Status = status
        };

        Validate(activity);
        await _repository.AddAsync(activity);

        return activity;
    }

    public async Task UpdateAsync(LearningActivity activity)
    {
        Validate(activity);
        await _repository.UpdateAsync(activity);
    }

    public Task DeleteAsync(int id)
    {
        return _repository.DeleteAsync(id);
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
