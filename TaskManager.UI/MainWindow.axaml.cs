using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace TaskManager.UI;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
