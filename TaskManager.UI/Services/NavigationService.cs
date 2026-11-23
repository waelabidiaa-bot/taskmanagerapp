using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Media;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.UI.ViewModels;

namespace TaskManager.UI.Services;

// Keeps the module registry and the one shell window. Navigating opens a module
// as a pane in that window (or focuses its pane when it is already open).
public class NavigationService : INavigationService
{
    private readonly IServiceProvider _services;
    private readonly Dictionary<string, object> _modules = new();
    private readonly Dictionary<string, Geometry?> _icons = new();
    private MainWindowViewModel? _shellViewModel;

    public NavigationService(IServiceProvider services)
    {
        _services = services;
    }

    public IReadOnlyList<(string Title, Geometry? Icon)> Modules =>
        _modules.Keys.Select(title => (title, _icons[title])).ToList();

    public object? CurrentViewModel { get; private set; }

    public MainWindow? ShellWindow { get; private set; }

    public void Register(string title, object viewModel, Geometry? icon = null)
    {
        _modules[title] = viewModel;
        _icons[title] = icon;
    }

    public async Task NavigateAsync(string module, object? parameter = null)
    {
        if (!_modules.TryGetValue(module, out var viewModel))
            return;

        EnsureShell();
        _shellViewModel!.OpenModule(module, _icons[module], viewModel);
        CurrentViewModel = viewModel;

        if (viewModel is INavigationAware navigationAware)
            await navigationAware.OnNavigatedToAsync(parameter);
    }

    private void EnsureShell()
    {
        if (ShellWindow is not null)
            return;

        var items = Modules.Select(module =>
        {
            var item = new NavigationItem(module.Title, module.Icon);
            item.SelectCommand = new AsyncRelayCommand(() => NavigateAsync(module.Title));
            return item;
        }).ToList();

        _shellViewModel = _services.GetRequiredService<MainWindowViewModel>();
        _shellViewModel.Initialize(items);

        ShellWindow = _services.GetRequiredService<MainWindow>();
        ShellWindow.DataContext = _shellViewModel;
        ShellWindow.Title = "My Workspace";
    }
}
