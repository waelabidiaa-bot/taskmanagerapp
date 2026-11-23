using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;

namespace TaskManager.UI.ViewModels;

public partial class NavigationItem : ObservableObject
{
    public NavigationItem(string title, object viewModel)
    {
        Title = title;
        ViewModel = viewModel;
    }

    public string Title { get; }
    public object ViewModel { get; }
    public ICommand? SelectCommand { get; set; }

    [ObservableProperty]
    private bool isSelected;
}
