using System;
using System.Windows.Input;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace TaskManager.UI.ViewModels;

// One panel in the split workspace: a header with a close button plus a module.
public partial class PaneViewModel : ObservableObject
{
    public PaneViewModel(string title, Geometry? icon, object viewModel, Action<PaneViewModel> close)
    {
        Title = title;
        Icon = icon;
        ViewModel = viewModel;
        CloseCommand = new RelayCommand(() => close(this));
    }

    public string Title { get; }
    public Geometry? Icon { get; }
    public object ViewModel { get; }
    public ICommand CloseCommand { get; }

    [ObservableProperty]
    private bool isActive;
}
