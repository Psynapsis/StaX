using FluentAvalonia.UI.Controls;
using ReactiveUI;
using System.Collections.ObjectModel;

namespace StaX.Desktop.Views;

internal class PluginLoaderVisualizator() : ReactiveObject
{
    private ObservableCollection<Symbol>? _icons = [];
    public ObservableCollection<Symbol>? Icons
    {
        get => _icons;
        set => this.RaiseAndSetIfChanged(ref _icons, value);
    }
}