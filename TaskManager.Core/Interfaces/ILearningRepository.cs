using TaskManager.Core.Models;

namespace TaskManager.Core.Interfaces;

public interface ILearningRepository
{
    Task<List<LearningActivity>> GetAllAsync();
    Task<LearningActivity?> GetByIdAsync(int id);
    Task AddAsync(LearningActivity activity);
    Task UpdateAsync(LearningActivity activity);
    Task DeleteAsync(int id);
    Task DeleteMilestoneAsync(int milestoneId);
}
