using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Media;

namespace TaskManager.UI.Services;

public interface INavigationService
{
    // Module metadata used to build each window's icon rail.
    IReadOnlyList<(string Title, Geometry? Icon)> Modules { get; }

    // The module most recently opened or focused (used by cross-module commands).
    object? CurrentViewModel { get; }

    void Register(string title, object viewModel, Geometry? icon = null);

    // Opens the module in its own window, or focuses it if already open.
    Task NavigateAsync(string module, object? parameter = null);
}
