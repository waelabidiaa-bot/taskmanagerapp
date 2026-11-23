using Microsoft.EntityFrameworkCore;
using TaskManager.Core.Interfaces;
using TaskManager.Core.Models;
using TaskManager.Data.Database;

namespace TaskManager.Data.Repositories;

public class LearningRepository : ILearningRepository
{
    private readonly ApplicationDbContext _context;

    public LearningRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<LearningActivity>> GetAllAsync()
    {
        return await _context.LearningActivities
            .AsNoTracking()
            .Include(l => l.Tasks)
            .Include(l => l.Notes)
            .OrderByDescending(l => l.CreatedAt)
            .ToListAsync();
    }

    public async Task<LearningActivity?> GetByIdAsync(int id)
    {
        return await _context.LearningActivities
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.Id == id);
    }

    public async Task AddAsync(LearningActivity activity)
    {
        await _context.LearningActivities.AddAsync(activity);
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();
    }

    public async Task UpdateAsync(LearningActivity activity)
    {
        _context.LearningActivities.Update(activity);
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();
    }

    public async Task DeleteAsync(int id)
    {
        var activity = await GetByIdAsync(id);
        if (activity is null)
            return;

        _context.LearningActivities.Remove(activity);
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();
    }
}
