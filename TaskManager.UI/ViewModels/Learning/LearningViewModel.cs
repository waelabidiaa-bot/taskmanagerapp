using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TaskManager.Core.Models;
using TaskManager.Core.Services;
using TaskManager.UI.Services;

namespace TaskManager.UI.ViewModels.Learning;

public partial class LearningViewModel : ObservableObject, INavigationAware
{
    private readonly LearningService _learningService;

    public LearningViewModel(LearningService learningService)
    {
        _learningService = learningService;
    }

    public ObservableCollection<LearningActivity> Activities { get; } = new();
    public Array Statuses { get; } = Enum.GetValues<LearningStatus>();

    [ObservableProperty] private string title = string.Empty;
    [ObservableProperty] private string description = string.Empty;
    [ObservableProperty] private LearningStatus status = LearningStatus.Planned;
    [ObservableProperty] private string errorMessage = string.Empty;
    [ObservableProperty] private LearningActivity? selectedActivity;
    [ObservableProperty] private string milestoneTitle = string.Empty;
    [ObservableProperty] private LearningMilestone? selectedMilestone;

    public async Task OnNavigatedToAsync(object? parameter = null)
    {
        Activities.Clear();
        foreach (var activity in await _learningService.GetAllAsync())
            Activities.Add(activity);
    }

    partial void OnSelectedActivityChanged(LearningActivity? value)
    {
        if (value is null)
        {
            ResetFormFields();
            return;
        }

        Title = value.Title;
        Description = value.Description;
        Status = value.Status;
        MilestoneTitle = string.Empty;
        SelectedMilestone = null;
        ErrorMessage = string.Empty;
    }

    [RelayCommand]
    private async Task SaveActivityAsync()
    {
        ErrorMessage = string.Empty;

        try
        {
            int savedId;

            if (SelectedActivity is null)
            {
                var created = await _learningService.CreateAsync(Title, Description, Status);
                savedId = created.Id;
            }
            else
            {
                SelectedActivity.Title = Title;
                SelectedActivity.Description = Description;
                SelectedActivity.Status = Status;
                savedId = SelectedActivity.Id;
                await _learningService.UpdateAsync(SelectedActivity);
            }

            await OnNavigatedToAsync();
            SelectedActivity = Activities.FirstOrDefault(a => a.Id == savedId);
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
        {
            ErrorMessage = ex.Message;
        }
    }

    [RelayCommand]
    private async Task AddMilestoneAsync()
    {
        if (SelectedActivity is null)
        {
            ErrorMessage = "Select a study goal first, then add milestones.";
            return;
        }

        ErrorMessage = string.Empty;

        try
        {
            await _learningService.AddMilestoneAsync(SelectedActivity.Id, MilestoneTitle);
            MilestoneTitle = string.Empty;
            await RefreshSelectedGoalAsync();
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
        {
            ErrorMessage = ex.Message;
        }
    }

    [RelayCommand]
    private async Task ToggleMilestoneAsync()
    {
        if (SelectedActivity is null || SelectedMilestone is null)
        {
            ErrorMessage = "Select a milestone first.";
            return;
        }

        ErrorMessage = string.Empty;
        await _learningService.ToggleMilestoneAsync(SelectedActivity.Id, SelectedMilestone.Id);
        SelectedMilestone = null;
        await RefreshSelectedGoalAsync();
    }

    [RelayCommand]
    private async Task DeleteMilestoneAsync()
    {
        if (SelectedActivity is null || SelectedMilestone is null)
        {
            ErrorMessage = "Select a milestone first.";
            return;
        }

        ErrorMessage = string.Empty;
        await _learningService.DeleteMilestoneAsync(SelectedActivity.Id, SelectedMilestone.Id);
        SelectedMilestone = null;
        await RefreshSelectedGoalAsync();
    }

    [RelayCommand]
    private async Task DeleteActivityAsync()
    {
        if (SelectedActivity is null)
            return;

        await _learningService.DeleteAsync(SelectedActivity.Id);
        SelectedActivity = null;
        await OnNavigatedToAsync();
    }

    [RelayCommand]
    private void ClearForm()
    {
        SelectedActivity = null;
        ResetFormFields();
    }

    private async Task RefreshSelectedGoalAsync()
    {
        var savedId = SelectedActivity?.Id;
        await OnNavigatedToAsync();
        SelectedActivity = Activities.FirstOrDefault(a => a.Id == savedId);
    }

    private void ResetFormFields()
    {
        Title = string.Empty;
        Description = string.Empty;
        Status = LearningStatus.Planned;
        MilestoneTitle = string.Empty;
        SelectedMilestone = null;
        ErrorMessage = string.Empty;
    }
}
