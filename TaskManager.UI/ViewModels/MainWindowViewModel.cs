using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;

namespace TaskManager.UI.ViewModels;

// ViewModel for the single window: the slim icon rail and the open panes.
public partial class MainWindowViewModel : ObservableObject
{
    public ObservableCollection<NavigationItem> NavigationItems { get; } = new();
    public ObservableCollection<PaneViewModel> Panes { get; } = new();

    [ObservableProperty]
    private object? currentViewModel;

    public void Initialize(IEnumerable<NavigationItem> items)
    {
        NavigationItems.Clear();
        foreach (var item in items)
            NavigationItems.Add(item);
    }

    public void OpenModule(string module, Geometry? icon, object viewModel)
    {
        var pane = Panes.FirstOrDefault(p => p.Title == module);
        if (pane is null)
        {
            pane = new PaneViewModel(module, icon, viewModel, ClosePane);
            Panes.Add(pane);
        }

        foreach (var openPane in Panes)
            openPane.IsActive = ReferenceEquals(openPane, pane);

        // Icons stay lit for every module that currently has a pane.
        foreach (var item in NavigationItems)
            item.IsSelected = Panes.Any(p => p.Title == item.Title);

        CurrentViewModel = viewModel;
    }

    private void ClosePane(PaneViewModel pane)
    {
        Panes.Remove(pane);

        foreach (var item in NavigationItems)
            item.IsSelected = Panes.Any(p => p.Title == item.Title);

        if (Panes.Count > 0)
            Panes[^1].IsActive = true;
    }
}
