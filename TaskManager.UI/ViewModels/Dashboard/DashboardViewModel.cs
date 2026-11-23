using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using TaskManager.Core.Models;
using TaskManager.Core.Services;
using TaskManager.UI.Services;

namespace TaskManager.UI.ViewModels.Dashboard;

public partial class DashboardViewModel : ObservableObject, INavigationAware
{
    private readonly TaskService _taskService;
    private readonly NoteService _noteService;
    private readonly LearningService _learningService;

    public DashboardViewModel(
        TaskService taskService,
        NoteService noteService,
        LearningService learningService)
    {
        _taskService = taskService;
        _noteService = noteService;
        _learningService = learningService;
    }

    [ObservableProperty]
    private int totalTasks;

    [ObservableProperty]
    private int completedTasks;

    [ObservableProperty]
    private int pendingTasks;

    public ObservableCollection<Note> RecentNotes { get; } = new();
    public ObservableCollection<LearningActivity> LearningActivities { get; } = new();

    public async Task OnNavigatedToAsync(object? parameter = null)
    {
        var tasks = await _taskService.GetAllAsync();
        TotalTasks = tasks.Count;
        CompletedTasks = tasks.Count(t => t.IsFinished);
        PendingTasks = TotalTasks - CompletedTasks;

        RecentNotes.Clear();
        foreach (var note in await _noteService.GetRecentAsync(5))
            RecentNotes.Add(note);

        LearningActivities.Clear();
        var activities = await _learningService.GetAllAsync();
        foreach (var activity in activities.Where(a => a.Status != LearningStatus.Completed).Take(5))
            LearningActivities.Add(activity);
    }
}
