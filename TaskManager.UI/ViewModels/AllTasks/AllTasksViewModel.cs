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

namespace TaskManager.UI.ViewModels.AllTasks;

// Read-focused module: one list with every task, and a full detail panel for the clicked task.
public partial class AllTasksViewModel : ObservableObject, INavigationAware
{
    private readonly TaskService _taskService;
    private readonly INavigationService _navigation;
    private List<TaskItem> _allTasks = new();

    public AllTasksViewModel(TaskService taskService, INavigationService navigation)
    {
        _taskService = taskService;
        _navigation = navigation;
    }

    public ObservableCollection<TaskItem> Tasks { get; } = new();
    public ObservableCollection<string> StatusFilters { get; } = new() { "All", "Pending", "Completed" };

    [ObservableProperty] private string searchText = string.Empty;
    [ObservableProperty] private string statusFilter = "All";
    [ObservableProperty] private TaskItem? selectedTask;
    [ObservableProperty] private int totalTasks;
    [ObservableProperty] private int completedTasks;
    [ObservableProperty] private int pendingTasks;
    [ObservableProperty] private int totalNotes;

    public async Task OnNavigatedToAsync(object? parameter = null)
    {
        _allTasks = await _taskService.GetAllAsync();

        TotalTasks = _allTasks.Count;
        CompletedTasks = _allTasks.Count(t => t.IsFinished);
        PendingTasks = TotalTasks - CompletedTasks;
        TotalNotes = _allTasks.Sum(t => t.Notes.Count);

        var previousId = SelectedTask?.Id;
        ApplyFilters();

        // Another module can open this page with a task to focus on.
        if (parameter is TaskItem incoming)
            SelectedTask = Tasks.FirstOrDefault(t => t.Id == incoming.Id) ?? incoming;
        else if (previousId is not null)
            SelectedTask = Tasks.FirstOrDefault(t => t.Id == previousId);
    }

    partial void OnSearchTextChanged(string value) => ApplyFilters();
    partial void OnStatusFilterChanged(string value) => ApplyFilters();

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
    private Task EditTaskAsync()
    {
        return SelectedTask is null
            ? Task.CompletedTask
            : _navigation.NavigateAsync("Tasks", SelectedTask);
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
                t.Description.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                t.Notes.Any(n => n.Title.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                                 n.Summary.Contains(search, StringComparison.OrdinalIgnoreCase)));
        }

        if (StatusFilter == "Pending")
            result = result.Where(t => !t.IsFinished);
        else if (StatusFilter == "Completed")
            result = result.Where(t => t.IsFinished);

        foreach (var task in result)
            Tasks.Add(task);
    }
}
