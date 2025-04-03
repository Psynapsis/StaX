using Avalonia.Controls;
using Avalonia.Threading;
using FluentAvalonia.UI.Controls;
using StaX.Desktop.Models;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace StaX.Desktop.Process;

public class LazyUiState
{
    public bool IsLoaded { get; private set; }

    public string StateName => _starter?.Name ?? string.Empty;

    public string ToolTip => _starter?.ToolTip ?? string.Empty;

    public Symbol? Icon => _starter?.Symbol ?? Symbol.ShareAndroid;

    public OutUiState? UiState { get; private set; }

    private readonly string _currentPluginFolder;

    private readonly Starter _starter;

    private TopLevel? _topLevel;

    private NativeHost? _nativeHost;

    public LazyUiState(string currentPluginFolder)
    {
        _currentPluginFolder = currentPluginFolder;
        _starter = TryLoadStarter(currentPluginFolder);
    }

    public async Task InitializeAsync()
        => await Dispatcher.UIThread.InvokeAsync(() =>
        {
            _topLevel ??= TopLevelWidget.GetInstance();
            var uiState = new OutUiState(_starter, _topLevel, _currentPluginFolder);
            uiState?.Load();

            if (_nativeHost is null)
            {
                var nativeEmbedPage = _topLevel.GetControl<NativeEmbedPage>("ChildPageHost");
                _nativeHost = nativeEmbedPage.GetControl<NativeHost>("ChildWindowHost");
            }

            _nativeHost.Implementation = uiState;

            IsLoaded = uiState is not null;
        });

    private static Starter? TryLoadStarter(string path)
    {
        try
        {
            var json = File.ReadAllText(Path.Combine(path, "start.json"));
            return JsonSerializer.Deserialize<Starter>(json);
        }
        catch
        {
            //inore
        }
        return null;
    }
}