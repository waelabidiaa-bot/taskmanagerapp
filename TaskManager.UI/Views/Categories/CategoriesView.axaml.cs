using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace TaskManager.UI.Views.Categories;

public partial class CategoriesView : UserControl
{
    public CategoriesView()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
