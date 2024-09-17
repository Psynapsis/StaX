using Avalonia.Threading;
using FluentAvalonia.UI.Controls;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading.Tasks;
using StaX.Domain;

namespace StaX.Desktop.Process;

public class LazyUiState
{
    public bool IsLoaded { get; private set; }

    public string StateName => _starter?.Name ?? UiState?.StateName ?? string.Empty;

    public string ToolTip => _starter?.ToolTip ?? UiState?.ToolTip ?? string.Empty;

    public Symbol? Icon => _starter?.Symbol ?? UiState?.Icon ?? Symbol.ShareAndroid;

    public IUiState? UiState { get; private set; }

    private readonly string _currentPluginFolder;

    private readonly Starter? _starter;

    public LazyUiState(IUiState state)
    {
        UiState = state;
        _currentPluginFolder = string.Empty;
        IsLoaded = true;
    }

    public LazyUiState(string currentPluginFolder)
    {
        _starter = TryLoadStarter(currentPluginFolder);
        _currentPluginFolder = currentPluginFolder;

        if (_starter is null)
            Initialize();
    }

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

    public async Task InitializeAsync()
        => await Dispatcher.UIThread.InvokeAsync(() =>
        {
            var pathToDll = Path.Combine(_currentPluginFolder, "Plugin", _starter.Implementer);
            var state = GetUiStateFromDll(pathToDll);
            if (state is not null)
                LoadNativeRuntimeDlls(Path.Combine(_currentPluginFolder, "Plugin"));

            UiState = state;
            IsLoaded = UiState is not null;

        });

    private void Initialize()
        => Dispatcher.UIThread.Invoke(() =>
        {
            var state = GetUiStateFromFolder(_currentPluginFolder);
            if (state is not null)
                LoadNativeRuntimeDlls(_currentPluginFolder);

            UiState = state;
            IsLoaded = UiState is not null;
        });

    private static IUiState? GetUiStateFromFolder(string path)
    {
        try
        {
            var dllFiles = Directory.GetFiles(path, "*.dll");
            if (dllFiles.Length == 0)
                dllFiles = Directory.GetFiles(Path.Combine(path, "Plugin"), "*.dll");

            foreach (var dllFile in dllFiles)
            {
                var uiState = GetUiStateFromDll(dllFile);
                if (uiState != null)
                    return uiState;
            }
        }
        catch
        {
            //ignore
        }

        return null;
    }

    private static IUiState? GetUiStateFromDll(string path)
    {
        try
        {
            var assembly = Assembly.LoadFrom(path);

            foreach (var type in assembly.GetTypes())
                if (typeof(IUiState).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
                {
                    var types = assembly.GetTypes().Where(x => typeof(IUiState).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract);
                    foreach (var thisType in types)
                    {
                        var uiStateObject = Activator.CreateInstance(thisType);

                        var uiState = uiStateObject as IUiState;

                        if (uiState is not null)
                            return uiState;
                    }
                }
        }
        catch
        {
        }
        return null;
    }

    private static void LoadNativeRuntimeDlls(string path)
    {
        var pathToNativeRuntimeDlls = Path.Combine(path, "runtimes", RuntimeInformation.RuntimeIdentifier, "native");
        if (Directory.Exists(pathToNativeRuntimeDlls))
            foreach (var file in Directory.EnumerateFiles(pathToNativeRuntimeDlls))
                try
                {
                    NativeLibrary.Load(file);
                }
                catch (Exception ex) {  /*not implemented*/ }
    }
}