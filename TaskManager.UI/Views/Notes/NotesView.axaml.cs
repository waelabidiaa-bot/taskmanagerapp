using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace TaskManager.UI.Views.Notes;

public partial class NotesView : UserControl
{
    public NotesView()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
