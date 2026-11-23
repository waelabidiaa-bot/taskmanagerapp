using TaskManager.Core.Interfaces;
using TaskManager.Core.Models;

namespace TaskManager.Core.Services;

public class CategoryService
{
    private readonly ICategoryRepository _repository;

    public CategoryService(ICategoryRepository repository)
    {
        _repository = repository;
    }

    public Task<List<Category>> GetAllAsync()
    {
        return _repository.GetAllAsync();
    }

    public async Task<Category> CreateAsync(string name)
    {
        var cleanedName = ValidateName(name);
        await EnsureNameIsFreeAsync(cleanedName);

        var category = new Category { Name = cleanedName };
        await _repository.AddAsync(category);

        return category;
    }

    public async Task RenameAsync(int id, string newName)
    {
        var cleanedName = ValidateName(newName);
        var category = await _repository.GetByIdAsync(id)
            ?? throw new InvalidOperationException("Category not found.");

        if (string.Equals(category.Name, cleanedName, StringComparison.OrdinalIgnoreCase))
            return;

        await EnsureNameIsFreeAsync(cleanedName);

        category.Name = cleanedName;
        await _repository.UpdateAsync(category);
    }

    public Task DeleteAsync(int id)
    {
        return _repository.DeleteAsync(id);
    }

    private async Task EnsureNameIsFreeAsync(string name)
    {
        var categories = await _repository.GetAllAsync();

        if (categories.Any(c => string.Equals(c.Name, name, StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException("A category with this name already exists.");
    }

    private static string ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Category name is required.");

        return name.Trim();
    }
}
