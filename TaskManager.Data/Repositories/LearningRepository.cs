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
            .Include(l => l.Milestones)
            .OrderByDescending(l => l.CreatedAt)
            .ToListAsync();
    }

    public async Task<LearningActivity?> GetByIdAsync(int id)
    {
        return await _context.LearningActivities
            .AsNoTracking()
            .Include(l => l.Milestones)
            .FirstOrDefaultAsync(l => l.Id == id);
    }

    public async Task AddAsync(LearningActivity activity)
    {
        await _context.LearningActivities.AddAsync(activity);
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();
    }

    // Also saves changes to the loaded Milestones collection, so the service can
    // add, toggle or remove milestones and update progress in one place.
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

    public async Task DeleteMilestoneAsync(int milestoneId)
    {
        var milestone = await _context.LearningMilestones.FindAsync(milestoneId);
        if (milestone is null)
            return;

        _context.LearningMilestones.Remove(milestone);
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();
    }
}
