using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace TaskManager.UI.Views.Shell;

public partial class PaneView : UserControl
{
    public PaneView()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
