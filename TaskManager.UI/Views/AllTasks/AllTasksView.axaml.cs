using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace TaskManager.UI.Views.AllTasks;

public partial class AllTasksView : UserControl
{
    public AllTasksView()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
