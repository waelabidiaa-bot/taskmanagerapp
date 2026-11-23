using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.Core.Interfaces;
using TaskManager.Core.Services;
using TaskManager.Data.Database;
using TaskManager.Data.Repositories;
using TaskManager.UI.Icons;
using TaskManager.UI.Services;
using TaskManager.UI.ViewModels;
using TaskManager.UI.ViewModels.AllTasks;
using TaskManager.UI.ViewModels.Categories;
using TaskManager.UI.ViewModels.Dashboard;
using TaskManager.UI.ViewModels.Learning;
using TaskManager.UI.ViewModels.Notes;
using TaskManager.UI.ViewModels.Settings;
using TaskManager.UI.ViewModels.Tasks;

namespace TaskManager.UI;

public partial class App : Application
{
    public static IServiceProvider Services { get; private set; } = null!;

    private IServiceScope? _appScope;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var services = new ServiceCollection();
        ConfigureServices(services);

        Services = services.BuildServiceProvider();
        _appScope = Services.CreateScope();

        // Apply pending EF Core migrations before any screen reads from the database.
        var context = _appScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        DatabaseConfiguration.InitializeAsync(context).GetAwaiter().GetResult();

        // Register the modules once; navigation opens them as panes in one window.
        var navigation = _appScope.ServiceProvider.GetRequiredService<NavigationService>();
        RegisterModules(navigation);

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            navigation.NavigateAsync("Dashboard").GetAwaiter().GetResult();
            desktop.MainWindow = navigation.ShellWindow;
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void RegisterModules(NavigationService navigation)
    {
        var provider = Services;
        navigation.Register("Dashboard", provider.GetRequiredService<DashboardViewModel>(), AppIcons.Dashboard);
        navigation.Register("All Tasks", provider.GetRequiredService<AllTasksViewModel>(), AppIcons.AllTasks);
        navigation.Register("Tasks", provider.GetRequiredService<TasksViewModel>(), AppIcons.Tasks);
        navigation.Register("Notes", provider.GetRequiredService<NotesViewModel>(), AppIcons.Notes);
        navigation.Register("Learning", provider.GetRequiredService<LearningViewModel>(), AppIcons.Learning);
        navigation.Register("Categories", provider.GetRequiredService<CategoriesViewModel>(), AppIcons.Categories);
        navigation.Register("Settings", provider.GetRequiredService<SettingsViewModel>(), AppIcons.Settings);
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        // Database
        services.AddDbContext<ApplicationDbContext>(DatabaseConfiguration.Configure);

        // Repositories
        services.AddScoped<ITaskRepository, TaskRepository>();
        services.AddScoped<INoteRepository, NoteRepository>();
        services.AddScoped<ILearningRepository, LearningRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();

        // Services
        services.AddScoped<TaskService>();
        services.AddScoped<NoteService>();
        services.AddScoped<LearningService>();
        services.AddScoped<CategoryService>();

        // Navigation (shared shell state)
        services.AddSingleton<NavigationService>();
        services.AddSingleton<INavigationService>(provider =>
            provider.GetRequiredService<NavigationService>());

        // A module has one pane, so its ViewModel is shared. The shell ViewModel
        // is transient so a fresh window gets a fresh rail and pane collection.
        services.AddTransient<MainWindow>();
        services.AddTransient<MainWindowViewModel>();
        services.AddSingleton<DashboardViewModel>();
        services.AddSingleton<AllTasksViewModel>();
        services.AddSingleton<TasksViewModel>();
        services.AddSingleton<NotesViewModel>();
        services.AddSingleton<LearningViewModel>();
        services.AddSingleton<CategoriesViewModel>();
        services.AddSingleton<SettingsViewModel>();
    }
}
