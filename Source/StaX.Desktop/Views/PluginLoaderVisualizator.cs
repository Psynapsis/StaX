using Avalonia.Collections;
using FluentAvalonia.UI.Controls;
using ReactiveUI;

namespace StaX.Desktop.Views;

internal class PluginLoaderVisualizator() : ReactiveObject
{
    private AvaloniaList<Symbol>? _icons = [];
    public AvaloniaList<Symbol>? Icons
    {
        get => _icons;
        set => this.RaiseAndSetIfChanged(ref _icons, value);
    }
}