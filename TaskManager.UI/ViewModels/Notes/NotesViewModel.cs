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

namespace TaskManager.UI.ViewModels.Notes;

public partial class NotesViewModel : ObservableObject, INavigationAware
{
    private readonly NoteService _noteService;
    private readonly CategoryService _categoryService;
    private readonly LearningService _learningService;
    private readonly TaskService _taskService;

    private List<Note> _allNotes = new();

    public NotesViewModel(
        NoteService noteService,
        CategoryService categoryService,
        LearningService learningService,
        TaskService taskService)
    {
        _noteService = noteService;
        _categoryService = categoryService;
        _learningService = learningService;
        _taskService = taskService;
    }

    public ObservableCollection<Note> Notes { get; } = new();
    public ObservableCollection<TaskItem> Tasks { get; } = new();
    public ObservableCollection<Category> Categories { get; } = new();
    public ObservableCollection<LearningActivity> LearningActivities { get; } = new();

    [ObservableProperty] private string title = string.Empty;
    [ObservableProperty] private string summary = string.Empty;
    [ObservableProperty] private string content = string.Empty;
    [ObservableProperty] private string searchText = string.Empty;
    [ObservableProperty] private string errorMessage = string.Empty;
    [ObservableProperty] private bool isPinned;
    [ObservableProperty] private Note? selectedNote;
    [ObservableProperty] private TaskItem? selectedTask;
    [ObservableProperty] private TaskItem? taskFilter;
    [ObservableProperty] private Category? selectedCategory;
    [ObservableProperty] private LearningActivity? selectedLearningActivity;

    public async Task OnNavigatedToAsync(object? parameter = null)
    {
        Tasks.Clear();
        var tasks = await _taskService.GetAllAsync();
        foreach (var task in tasks)
            Tasks.Add(task);

        // Opening Notes from a task pre-selects that task and filters the notes list.
        TaskFilter = parameter as TaskItem is { } incoming
            ? Tasks.FirstOrDefault(t => t.Id == incoming.Id) ?? incoming
            : null;
        SelectedTask = TaskFilter;

        _allNotes = await _noteService.GetAllAsync();
        ApplyFilters();

        Categories.Clear();
        foreach (var category in await _categoryService.GetAllAsync())
            Categories.Add(category);

        LearningActivities.Clear();
        foreach (var activity in await _learningService.GetAllAsync())
            LearningActivities.Add(activity);
    }

    partial void OnSelectedNoteChanged(Note? value)
    {
        if (value is null)
        {
            ResetFormFields();
            return;
        }

        Title = value.Title;
        Summary = value.Summary;
        Content = value.Content;
        IsPinned = value.IsPinned;
        SelectedTask = Tasks.FirstOrDefault(t => t.Id == value.TaskId);
        SelectedCategory = Categories.FirstOrDefault(c => c.Id == value.CategoryId);
        SelectedLearningActivity = LearningActivities.FirstOrDefault(l => l.Id == value.LearningActivityId);
        ErrorMessage = string.Empty;
    }

    partial void OnSearchTextChanged(string value) => ApplyFilters();

    [RelayCommand]
    private async Task SaveNoteAsync()
    {
        ErrorMessage = string.Empty;

        try
        {
            if (SelectedNote is null)
            {
                await _noteService.CreateAsync(
                    Title,
                    Content,
                    IsPinned,
                    SelectedCategory?.Id,
                    SelectedLearningActivity?.Id,
                    Summary,
                    SelectedTask?.Id);
            }
            else
            {
                SelectedNote.Title = Title;
                SelectedNote.Summary = Summary;
                SelectedNote.Content = Content;
                SelectedNote.IsPinned = IsPinned;
                SelectedNote.TaskId = SelectedTask?.Id;
                SelectedNote.CategoryId = SelectedCategory?.Id;
                SelectedNote.LearningActivityId = SelectedLearningActivity?.Id;
                await _noteService.UpdateAsync(SelectedNote);
            }

            SelectedNote = null;
            await OnNavigatedToAsync(TaskFilter);
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
        {
            ErrorMessage = ex.Message;
        }
    }

    [RelayCommand]
    private async Task TogglePinAsync()
    {
        if (SelectedNote is null)
            return;

        await _noteService.TogglePinAsync(SelectedNote.Id);
        SelectedNote = null;
        await OnNavigatedToAsync(TaskFilter);
    }

    [RelayCommand]
    private async Task DeleteNoteAsync()
    {
        if (SelectedNote is null)
            return;

        await _noteService.DeleteAsync(SelectedNote.Id);
        SelectedNote = null;
        await OnNavigatedToAsync(TaskFilter);
    }

    [RelayCommand]
    private void ClearForm()
    {
        SelectedNote = null;
        Title = string.Empty;
        Summary = string.Empty;
        Content = string.Empty;
        IsPinned = false;
        SelectedCategory = null;
        SelectedLearningActivity = null;
        // Keep the task filter so a new note stays attached to the same task.
        SelectedTask = TaskFilter;
        ErrorMessage = string.Empty;
    }

    [RelayCommand]
    private void ClearTaskFilter()
    {
        TaskFilter = null;
        SelectedTask = null;
        ApplyFilters();
    }

    private void ResetFormFields()
    {
        Title = string.Empty;
        Summary = string.Empty;
        Content = string.Empty;
        IsPinned = false;
        SelectedTask = TaskFilter;
        SelectedCategory = null;
        SelectedLearningActivity = null;
        ErrorMessage = string.Empty;
    }

    private void ApplyFilters()
    {
        Notes.Clear();

        var result = _allNotes.AsEnumerable();

        if (TaskFilter is not null)
            result = result.Where(n => n.TaskId == TaskFilter.Id);

        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            var search = SearchText.Trim();
            result = result.Where(n =>
                n.Title.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                n.Summary.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                n.Content.Contains(search, StringComparison.OrdinalIgnoreCase));
        }

        foreach (var note in result)
            Notes.Add(note);
    }
}
