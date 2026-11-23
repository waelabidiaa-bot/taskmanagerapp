using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace TaskManager.UI.Views.Dashboard;

public partial class DashboardView : UserControl
{
    public DashboardView()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
