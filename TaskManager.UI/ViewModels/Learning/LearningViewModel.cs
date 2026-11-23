using System;
using System.Collections.ObjectModel;
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
    [ObservableProperty] private double progressInput;
    [ObservableProperty] private LearningStatus status = LearningStatus.Planned;
    [ObservableProperty] private string errorMessage = string.Empty;
    [ObservableProperty] private LearningActivity? selectedActivity;

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
        ProgressInput = value.Progress;
        Status = value.Status;
        ErrorMessage = string.Empty;
    }

    [RelayCommand]
    private async Task SaveActivityAsync()
    {
        ErrorMessage = string.Empty;
        var progress = (int)Math.Round(ProgressInput);

        try
        {
            if (SelectedActivity is null)
            {
                await _learningService.CreateAsync(Title, Description, progress, Status);
            }
            else
            {
                SelectedActivity.Title = Title;
                SelectedActivity.Description = Description;
                SelectedActivity.Progress = progress;
                SelectedActivity.Status = Status;
                await _learningService.UpdateAsync(SelectedActivity);
            }

            SelectedActivity = null;
            await OnNavigatedToAsync();
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
        {
            ErrorMessage = ex.Message;
        }
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

    private void ResetFormFields()
    {
        Title = string.Empty;
        Description = string.Empty;
        ProgressInput = 0;
        Status = LearningStatus.Planned;
        ErrorMessage = string.Empty;
    }
}
