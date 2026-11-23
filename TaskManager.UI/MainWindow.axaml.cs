using System;
using System.Collections.Specialized;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using TaskManager.UI.ViewModels;
using TaskManager.UI.Views.Shell;

namespace TaskManager.UI;

public partial class MainWindow : Window
{
    private MainWindowViewModel? _boundViewModel;

    public MainWindow()
    {
        AvaloniaXamlLoader.Load(this);
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);

        if (_boundViewModel is not null)
            _boundViewModel.Panes.CollectionChanged -= OnPanesChanged;

        _boundViewModel = DataContext as MainWindowViewModel;

        if (_boundViewModel is not null)
            _boundViewModel.Panes.CollectionChanged += OnPanesChanged;

        RebuildPanes();
    }

    // Layout-only code: turn the open panes into grid columns with splitters.
    private void OnPanesChanged(object? sender, NotifyCollectionChangedEventArgs e) => RebuildPanes();

    private void RebuildPanes()
    {
        var grid = this.FindControl<Grid>("PaneGrid");
        if (grid is null)
            return;

        grid.Children.Clear();
        grid.ColumnDefinitions.Clear();

        var panes = _boundViewModel?.Panes;
        if (panes is null)
            return;

        for (var i = 0; i < panes.Count; i++)
        {
            if (i > 0)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
                var splitter = new GridSplitter
                {
                    Width = 6,
                    ResizeDirection = GridResizeDirection.Columns,
                    ResizeBehavior = GridResizeBehavior.PreviousAndNext,
                    VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch,
                    Background = new SolidColorBrush(Color.Parse("#334155"))
                };
                Grid.SetColumn(splitter, i * 2 - 1);
                grid.Children.Add(splitter);
            }

            grid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(1, GridUnitType.Star))
            {
                MinWidth = 260
            });
            var pane = new PaneView { DataContext = panes[i] };
            Grid.SetColumn(pane, i * 2);
            grid.Children.Add(pane);
        }
    }
}
