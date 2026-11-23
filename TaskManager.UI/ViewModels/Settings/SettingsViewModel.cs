using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TaskManager.Core.Services;
using TaskManager.Data.Database;
using TaskManager.UI.Services;

namespace TaskManager.UI.ViewModels.Settings;

public partial class SettingsViewModel : ObservableObject, INavigationAware
{
    private readonly TaskService _taskService;
    private readonly NoteService _noteService;
    private readonly LearningService _learningService;
    private readonly CategoryService _categoryService;

    public SettingsViewModel(
        TaskService taskService,
        NoteService noteService,
        LearningService learningService,
        CategoryService categoryService)
    {
        _taskService = taskService;
        _noteService = noteService;
        _learningService = learningService;
        _categoryService = categoryService;
    }

    public string DatabaseFile => DatabaseConfiguration.ConnectionString.Replace("Data Source=", string.Empty);

    [ObservableProperty] private int taskCount;
    [ObservableProperty] private int noteCount;
    [ObservableProperty] private int learningCount;
    [ObservableProperty] private int categoryCount;

    public async Task OnNavigatedToAsync(object? parameter = null)
    {
        TaskCount = (await _taskService.GetAllAsync()).Count;
        NoteCount = (await _noteService.GetAllAsync()).Count;
        LearningCount = (await _learningService.GetAllAsync()).Count;
        CategoryCount = (await _categoryService.GetAllAsync()).Count;
    }

    [RelayCommand]
    private Task RefreshAsync() => OnNavigatedToAsync();
}
