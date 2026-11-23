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
using TaskManager.UI.Services;
using TaskManager.UI.ViewModels;
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

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var mainWindow = _appScope.ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.DataContext = _appScope.ServiceProvider
                .GetRequiredService<MainWindowViewModel>();
            desktop.MainWindow = mainWindow;
        }

        base.OnFrameworkInitializationCompleted();
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

        // ViewModels and the shell
        services.AddTransient<MainWindow>();
        services.AddTransient<MainWindowViewModel>();
        services.AddTransient<DashboardViewModel>();
        services.AddTransient<TasksViewModel>();
        services.AddTransient<NotesViewModel>();
        services.AddTransient<LearningViewModel>();
        services.AddTransient<CategoriesViewModel>();
        services.AddTransient<SettingsViewModel>();
    }
}
