using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TaskManager.Core.Models;
using TaskManager.Core.Services;
using TaskManager.UI.Services;

namespace TaskManager.UI.ViewModels.Categories;

public partial class CategoriesViewModel : ObservableObject, INavigationAware
{
    private readonly CategoryService _categoryService;

    public CategoriesViewModel(CategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    public ObservableCollection<Category> Categories { get; } = new();

    [ObservableProperty] private string name = string.Empty;
    [ObservableProperty] private string errorMessage = string.Empty;
    [ObservableProperty] private Category? selectedCategory;

    public async Task OnNavigatedToAsync(object? parameter = null)
    {
        Categories.Clear();
        foreach (var category in await _categoryService.GetAllAsync())
            Categories.Add(category);
    }

    partial void OnSelectedCategoryChanged(Category? value)
    {
        Name = value?.Name ?? string.Empty;
        ErrorMessage = string.Empty;
    }

    [RelayCommand]
    private async Task SaveCategoryAsync()
    {
        ErrorMessage = string.Empty;

        try
        {
            if (SelectedCategory is null)
                await _categoryService.CreateAsync(Name);
            else
                await _categoryService.RenameAsync(SelectedCategory.Id, Name);

            SelectedCategory = null;
            await OnNavigatedToAsync();
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
        {
            ErrorMessage = ex.Message;
        }
    }

    [RelayCommand]
    private async Task DeleteCategoryAsync()
    {
        if (SelectedCategory is null)
            return;

        await _categoryService.DeleteAsync(SelectedCategory.Id);
        SelectedCategory = null;
        await OnNavigatedToAsync();
    }

    [RelayCommand]
    private void ClearForm()
    {
        SelectedCategory = null;
        Name = string.Empty;
        ErrorMessage = string.Empty;
    }
}
