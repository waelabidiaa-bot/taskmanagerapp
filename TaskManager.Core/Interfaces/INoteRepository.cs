using TaskManager.Core.Models;

namespace TaskManager.Core.Interfaces;

public interface INoteRepository
{
    Task<List<Note>> GetAllAsync();
    Task<List<Note>> GetRecentAsync(int count);
    Task<Note?> GetByIdAsync(int id);
    Task AddAsync(Note note);
    Task UpdateAsync(Note note);
    Task DeleteAsync(int id);
}
