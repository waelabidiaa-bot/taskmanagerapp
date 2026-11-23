using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace TaskManager.UI.Views.Settings;

public partial class SettingsView : UserControl
{
    public SettingsView()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
