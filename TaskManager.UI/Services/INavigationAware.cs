using System.Threading.Tasks;

namespace TaskManager.UI.Services;

// Modules implement this when they need to refresh data after becoming visible.
// The parameter lets another module pass context, for example a task whose notes should be shown.
public interface INavigationAware
{
    Task OnNavigatedToAsync(object? parameter = null);
}
