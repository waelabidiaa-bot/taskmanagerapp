using TaskManager.Core.Interfaces;
using TaskManager.Core.Models;

namespace TaskManager.Core.Services;

public class NoteService
{
    private readonly INoteRepository _repository;

    public NoteService(INoteRepository repository)
    {
        _repository = repository;
    }

    public Task<List<Note>> GetAllAsync()
    {
        return _repository.GetAllAsync();
    }

    public Task<List<Note>> GetRecentAsync(int count)
    {
        return _repository.GetRecentAsync(count);
    }

    public async Task<Note> CreateAsync(
        string title,
        string content,
        bool isPinned = false,
        int? categoryId = null,
        int? learningActivityId = null,
        string summary = "",
        int? taskId = null)
    {
        var note = new Note
        {
            Title = title,
            Content = content,
            IsPinned = isPinned,
            CategoryId = categoryId,
            LearningActivityId = learningActivityId,
            Summary = summary,
            TaskId = taskId
        };

        Validate(note);
        await _repository.AddAsync(note);

        return note;
    }

    public async Task UpdateAsync(Note note)
    {
        note.UpdatedAt = DateTime.UtcNow;
        Validate(note);
        await _repository.UpdateAsync(note);
    }

    public async Task TogglePinAsync(int id)
    {
        var note = await _repository.GetByIdAsync(id);
        if (note is null)
            return;

        note.IsPinned = !note.IsPinned;
        note.UpdatedAt = DateTime.UtcNow;
        await _repository.UpdateAsync(note);
    }

    public Task DeleteAsync(int id)
    {
        return _repository.DeleteAsync(id);
    }

    private static void Validate(Note note)
    {
        if (string.IsNullOrWhiteSpace(note.Title))
            throw new ArgumentException("Title is required.");

        note.Title = note.Title.Trim();
        note.Content ??= string.Empty;
        note.Summary ??= string.Empty;
    }
}
