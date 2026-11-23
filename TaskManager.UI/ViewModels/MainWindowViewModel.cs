using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using TaskManager.UI.Services;
using TaskManager.UI.ViewModels.Categories;
using TaskManager.UI.ViewModels.Dashboard;
using TaskManager.UI.ViewModels.Learning;
using TaskManager.UI.ViewModels.Notes;
using TaskManager.UI.ViewModels.Settings;
using TaskManager.UI.ViewModels.Tasks;

namespace TaskManager.UI.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    private readonly INavigationService _navigation;

    public ObservableCollection<NavigationItem> NavigationItems => _navigation.NavigationItems;
    public object? CurrentViewModel => _navigation.CurrentViewModel;

    public MainWindowViewModel(
        INavigationService navigation,
        DashboardViewModel dashboard,
        TasksViewModel tasks,
        NotesViewModel notes,
        LearningViewModel learning,
        CategoriesViewModel categories,
        SettingsViewModel settings)
    {
        _navigation = navigation;

        navigation.Register("Dashboard", dashboard);
        navigation.Register("Tasks", tasks);
        navigation.Register("Notes", notes);
        navigation.Register("Learning", learning);
        navigation.Register("Categories", categories);
        navigation.Register("Settings", settings);

        navigation.Navigated += () => OnPropertyChanged(nameof(CurrentViewModel));
        _ = navigation.NavigateAsync("Dashboard");
    }
}
