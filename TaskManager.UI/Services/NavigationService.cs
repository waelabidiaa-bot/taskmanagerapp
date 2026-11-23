using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using TaskManager.UI.ViewModels;

namespace TaskManager.UI.Services;

// Small shared shell service: keeps the list of modules and switches the visible one.
public class NavigationService : INavigationService
{
    public ObservableCollection<NavigationItem> NavigationItems { get; } = new();
    public object? CurrentViewModel { get; private set; }
    public event Action? Navigated;

    public void Register(string title, object viewModel)
    {
        var item = new NavigationItem(title, viewModel);
        item.SelectCommand = new AsyncRelayCommand(() => NavigateAsync(title));
        NavigationItems.Add(item);
    }

    public async Task NavigateAsync(string module, object? parameter = null)
    {
        var item = NavigationItems.FirstOrDefault(i => i.Title == module);
        if (item is null)
            return;

        foreach (var navigationItem in NavigationItems)
            navigationItem.IsSelected = ReferenceEquals(navigationItem, item);

        CurrentViewModel = item.ViewModel;
        Navigated?.Invoke();

        if (item.ViewModel is INavigationAware navigationAware)
            await navigationAware.OnNavigatedToAsync(parameter);
    }
}
