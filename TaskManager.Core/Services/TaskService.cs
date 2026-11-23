using TaskManager.Core.Interfaces;
using TaskManager.Core.Models;

namespace TaskManager.Core.Services;

public class TaskService
{
    private readonly ITaskRepository _repository;

    public TaskService(ITaskRepository repository)
    {
        _repository = repository;
    }

    public Task<List<TaskItem>> GetAllAsync()
    {
        return _repository.GetAllAsync();
    }

    public async Task<TaskItem> CreateAsync(
        string title,
        string description,
        TaskPriority priority = TaskPriority.Medium,
        DateTime? dueDate = null,
        int? categoryId = null,
        int? learningActivityId = null)
    {
        var task = new TaskItem
        {
            Title = title,
            Description = description,
            Priority = priority,
            DueDate = dueDate,
            CategoryId = categoryId,
            LearningActivityId = learningActivityId
        };

        Validate(task);
        await _repository.AddAsync(task);

        return task;
    }

    public async Task UpdateAsync(TaskItem task)
    {
        Validate(task);
        await _repository.UpdateAsync(task);
    }

    public Task DeleteAsync(int id)
    {
        return _repository.DeleteAsync(id);
    }

    private static void Validate(TaskItem task)
    {
        if (string.IsNullOrWhiteSpace(task.Title))
            throw new ArgumentException("Title is required.");

        task.Title = task.Title.Trim();
        task.Description ??= string.Empty;
    }
}
