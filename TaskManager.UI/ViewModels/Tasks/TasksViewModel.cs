using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TaskManager.Core.Models;
using TaskManager.Core.Services;
using TaskManager.UI.Services;

namespace TaskManager.UI.ViewModels.Tasks;

public partial class TasksViewModel : ObservableObject, INavigationAware
{
    private readonly TaskService _taskService;
    private readonly CategoryService _categoryService;
    private readonly LearningService _learningService;
    private readonly INavigationService _navigation;

    private List<TaskItem> _allTasks = new();

    public TasksViewModel(
        TaskService taskService,
        CategoryService categoryService,
        LearningService learningService,
        INavigationService navigation)
    {
        _taskService = taskService;
        _categoryService = categoryService;
        _learningService = learningService;
        _navigation = navigation;
    }

    public ObservableCollection<TaskItem> Tasks { get; } = new();
    public ObservableCollection<Category> Categories { get; } = new();
    public ObservableCollection<LearningActivity> LearningActivities { get; } = new();
    public ObservableCollection<string> StatusFilters { get; } = new() { "All", "Pending", "Completed" };
    public ObservableCollection<string> PriorityFilters { get; } = new() { "All", "Low", "Medium", "High" };
    public Array Priorities { get; } = Enum.GetValues<TaskPriority>();

    [ObservableProperty] private string title = string.Empty;
    [ObservableProperty] private string description = string.Empty;
    [ObservableProperty] private string searchText = string.Empty;
    [ObservableProperty] private string errorMessage = string.Empty;
    [ObservableProperty] private string statusFilter = "All";
    [ObservableProperty] private string priorityFilter = "All";
    [ObservableProperty] private TaskItem? selectedTask;
    [ObservableProperty] private TaskPriority priority = TaskPriority.Medium;
    [ObservableProperty] private DateTimeOffset? dueDate;
    [ObservableProperty] private Category? selectedCategory;
    [ObservableProperty] private LearningActivity? selectedLearningActivity;

    public async Task OnNavigatedToAsync(object? parameter = null)
    {
        _allTasks = await _taskService.GetAllAsync();
        ApplyFilters();

        Categories.Clear();
        foreach (var category in await _categoryService.GetAllAsync())
            Categories.Add(category);

        LearningActivities.Clear();
        foreach (var activity in await _learningService.GetAllAsync())
            LearningActivities.Add(activity);
    }

    partial void OnSelectedTaskChanged(TaskItem? value)
    {
        if (value is null)
        {
            ResetFormFields();
            return;
        }

        Title = value.Title;
        Description = value.Description;
        Priority = value.Priority;
        DueDate = value.DueDate.HasValue ? new DateTimeOffset(value.DueDate.Value, TimeSpan.Zero) : null;
        SelectedCategory = Categories.FirstOrDefault(c => c.Id == value.CategoryId);
        SelectedLearningActivity = LearningActivities.FirstOrDefault(l => l.Id == value.LearningActivityId);
        ErrorMessage = string.Empty;
    }

    partial void OnSearchTextChanged(string value) => ApplyFilters();
    partial void OnStatusFilterChanged(string value) => ApplyFilters();
    partial void OnPriorityFilterChanged(string value) => ApplyFilters();

    [RelayCommand]
    private async Task SaveTaskAsync()
    {
        ErrorMessage = string.Empty;

        try
        {
            if (SelectedTask is null)
            {
                await _taskService.CreateAsync(
                    Title,
                    Description,
                    Priority,
                    DueDate?.UtcDateTime,
                    SelectedCategory?.Id,
                    SelectedLearningActivity?.Id);
            }
            else
            {
                SelectedTask.Title = Title;
                SelectedTask.Description = Description;
                SelectedTask.Priority = Priority;
                SelectedTask.DueDate = DueDate?.UtcDateTime;
                SelectedTask.CategoryId = SelectedCategory?.Id;
                SelectedTask.LearningActivityId = SelectedLearningActivity?.Id;
                await _taskService.UpdateAsync(SelectedTask);
            }

            SelectedTask = null;
            await OnNavigatedToAsync();
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
        {
            ErrorMessage = ex.Message;
        }
    }

    [RelayCommand]
    private async Task ToggleCompleteAsync()
    {
        if (SelectedTask is null)
            return;

        SelectedTask.IsFinished = !SelectedTask.IsFinished;
        await _taskService.UpdateAsync(SelectedTask);
        await OnNavigatedToAsync();
    }

    [RelayCommand]
    private Task OpenTaskNotesAsync()
    {
        return SelectedTask is null
            ? Task.CompletedTask
            : _navigation.NavigateAsync("Notes", SelectedTask);
    }

    [RelayCommand]
    private async Task DeleteTaskAsync()
    {
        if (SelectedTask is null)
            return;

        await _taskService.DeleteAsync(SelectedTask.Id);
        SelectedTask = null;
        await OnNavigatedToAsync();
    }

    [RelayCommand]
    private void ClearForm()
    {
        SelectedTask = null;
        ResetFormFields();
    }

    private void ResetFormFields()
    {
        Title = string.Empty;
        Description = string.Empty;
        Priority = TaskPriority.Medium;
        DueDate = null;
        SelectedCategory = null;
        SelectedLearningActivity = null;
        ErrorMessage = string.Empty;
    }

    private void ApplyFilters()
    {
        Tasks.Clear();

        var result = _allTasks.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            var search = SearchText.Trim();
            result = result.Where(t =>
                t.Title.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                t.Description.Contains(search, StringComparison.OrdinalIgnoreCase));
        }

        if (StatusFilter == "Pending")
            result = result.Where(t => !t.IsFinished);
        else if (StatusFilter == "Completed")
            result = result.Where(t => t.IsFinished);

        if (PriorityFilter != "All" && Enum.TryParse<TaskPriority>(PriorityFilter, out var priority))
            result = result.Where(t => t.Priority == priority);

        foreach (var task in result)
            Tasks.Add(task);
    }
}
