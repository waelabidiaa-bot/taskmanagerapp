using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using TaskManager.UI.ViewModels;

namespace TaskManager.UI.Services;

public interface INavigationService
{
    ObservableCollection<NavigationItem> NavigationItems { get; }
    object? CurrentViewModel { get; }
    void Register(string title, object viewModel);
    event Action? Navigated;
    Task NavigateAsync(string module, object? parameter = null);
}
