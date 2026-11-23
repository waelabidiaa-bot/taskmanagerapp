using Microsoft.EntityFrameworkCore;
using TaskManager.Core.Interfaces;
using TaskManager.Core.Models;
using TaskManager.Data.Database;

namespace TaskManager.Data.Repositories;

public class TaskRepository : ITaskRepository
{
    private readonly ApplicationDbContext _context;

    public TaskRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<TaskItem>> GetAllAsync()
    {
        return await _context.Tasks
            .AsNoTracking()
            .Include(t => t.Category)
            .Include(t => t.LearningActivity)
            .Include(t => t.Notes)
            .OrderBy(t => t.IsFinished)
            .ThenByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<TaskItem?> GetByIdAsync(int id)
    {
        return await _context.Tasks
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task AddAsync(TaskItem task)
    {
        await _context.Tasks.AddAsync(task);
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();
    }

    public async Task UpdateAsync(TaskItem task)
    {
        _context.Tasks.Update(task);
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();
    }

    public async Task DeleteAsync(int id)
    {
        var task = await GetByIdAsync(id);
        if (task is null)
            return;

        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();
    }
}
