using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace TaskManager.UI.Views.Tasks;

public partial class TasksView : UserControl
{
    public TasksView()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
