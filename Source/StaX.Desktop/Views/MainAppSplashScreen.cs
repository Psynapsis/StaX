using Avalonia.Media;
using FluentAvalonia.UI.Windowing;
using ReactiveUI;
using ReactiveUI.Validation.Helpers;
using StaX.Desktop.Models;
using StaX.Desktop.ViewModels;
using System;
using System.Collections.Specialized;
using System.Threading;
using System.Threading.Tasks;

namespace StaX.Desktop.Views;

internal class MainAppSplashScreen(StartupArgs args, MainWindowViewModel mainWindowViewModel)
    : ReactiveValidationObject, IApplicationSplashScreen
{
    private readonly ServiceLocator _serviceLocator = new();

    private PluginLoaderVisualizator _pluginLoaderVisualizator = new();
    public PluginLoaderVisualizator PluginLoaderVisualizator
    {
        get => _pluginLoaderVisualizator;
        set => this.RaiseAndSetIfChanged(ref _pluginLoaderVisualizator, value);
    }

    public string AppName { get; } = string.Empty;
    public IImage? AppIcon { get; }

    public object SplashScreenContent => new MainAppSplashContent()
    {
        DataContext = this,
    };

    public int MinimumShowTime => 1200;

    public Action? InitApp { get; set; }

    public async Task RunTasks(CancellationToken cancellationToken)
    {
        InitApp ??= AppInitAsync();
        await Task.Run(InitApp, cancellationToken);
    }

    private Action AppInitAsync() =>
        async () =>
        {
            void _icons_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
                => this.RaisePropertyChanged(nameof(PluginLoaderVisualizator.Icons));


            if (_pluginLoaderVisualizator is not null && PluginLoaderVisualizator.Icons is not null)
            {
                PluginLoaderVisualizator.Icons.CollectionChanged += _icons_CollectionChanged;
                await _serviceLocator.RegisterAsync(_pluginLoaderVisualizator, args);
            }

            await mainWindowViewModel.StartProcessAsync();
        };
}