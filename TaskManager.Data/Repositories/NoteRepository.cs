using Microsoft.EntityFrameworkCore;
using TaskManager.Core.Interfaces;
using TaskManager.Core.Models;
using TaskManager.Data.Database;

namespace TaskManager.Data.Repositories;

public class NoteRepository : INoteRepository
{
    private readonly ApplicationDbContext _context;

    public NoteRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Note>> GetAllAsync()
    {
        return await _context.Notes
            .AsNoTracking()
            .Include(n => n.Task)
            .Include(n => n.Category)
            .Include(n => n.LearningActivity)
            .OrderByDescending(n => n.IsPinned)
            .ThenByDescending(n => n.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<Note>> GetRecentAsync(int count)
    {
        return await _context.Notes
            .AsNoTracking()
            .Include(n => n.Task)
            .Include(n => n.Category)
            .OrderByDescending(n => n.CreatedAt)
            .Take(count)
            .ToListAsync();
    }

    public async Task<Note?> GetByIdAsync(int id)
    {
        return await _context.Notes
            .AsNoTracking()
            .FirstOrDefaultAsync(n => n.Id == id);
    }

    public async Task AddAsync(Note note)
    {
        await _context.Notes.AddAsync(note);
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();
    }

    public async Task UpdateAsync(Note note)
    {
        _context.Notes.Update(note);
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();
    }

    public async Task DeleteAsync(int id)
    {
        var note = await GetByIdAsync(id);
        if (note is null)
            return;

        _context.Notes.Remove(note);
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();
    }
}
