using System.Windows.Input;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;

namespace TaskManager.UI.ViewModels;

// One button in the slim icon rail. Each window owns its own items so the
// highlighted icon reflects the module shown in that window.
public partial class NavigationItem : ObservableObject
{
    public NavigationItem(string title, Geometry? icon = null)
    {
        Title = title;
        Icon = icon;
    }

    public string Title { get; }
    public Geometry? Icon { get; }
    public ICommand? SelectCommand { get; set; }

    [ObservableProperty]
    private bool isSelected;
}
