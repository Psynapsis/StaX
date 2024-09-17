using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using Splat;
using StaX.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace StaX.Debug;

public partial class App : Application
{
    [DllImport("kernel32", SetLastError = true)]
    static extern bool SetDllDirectory(string lpPathName);

    public App()
    {
    }

    public App(string[] args)
    {
        var uiState = LoadPlugin(args[0]);

        if (uiState != null)
            _uiState = uiState;
        else
            throw new NotImplementedException();
    }

    private readonly IUiState? _uiState;

    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    public override void OnFrameworkInitializationCompleted()
    {
        base.OnFrameworkInitializationCompleted();

        if (_uiState is not null)
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                desktop.MainWindow = new MainWindow(_uiState);

            if (_uiState is UiState state)
                Dispatcher.UIThread.Invoke(() => state.Initialize(Locator.Current, TopLevelWidget.GetInstance()));

            EntryAction(_uiState);
        }
    }

    private static async void EntryAction(IUiState uiState) => await uiState.StateViewModel.EntryActionAsync();

    private static void LoadNativeRuntimeDlls(string path)
    {
        var pathToNativeRuntimeDlls = Path.Combine(path, "runtimes", RuntimeInformation.RuntimeIdentifier, "native");
        if (Directory.Exists(pathToNativeRuntimeDlls))
            foreach (var file in Directory.EnumerateFiles(pathToNativeRuntimeDlls))
                try
                {
                    NativeLibrary.Load(file);
                }
                catch {  /*not implemented*/ }

        var files = Directory.GetFiles(path);
        foreach (var file in files)
            try
            {
                Assembly.LoadFile(file);
            }
            catch {  /*not implemented*/ }
    }

    private static IUiState? LoadPlugin(string path)
    {
        var fullPath = Path.Combine(Environment.CurrentDirectory, path);
        List<IUiState> states = [];

        var pluginFolder = Directory.GetParent(fullPath);

        if (pluginFolder is null)
            return null;

        SetDllDirectory(pluginFolder.FullName);

        using (var catalog = new DirectoryCatalog(pluginFolder.FullName))
        using (var container = new CompositionContainer(catalog))
        {
            var unknownStates = GetAllTypesThatImplementInterface<IUiState>(fullPath);

            if (unknownStates is not null && unknownStates.Count() > 0)
            {
                LoadNativeRuntimeDlls(pluginFolder.FullName);
                foreach (var stateType in unknownStates)
                {
                    try
                    {
                        var stateInstance = Activator.CreateInstance(stateType);
                        if (stateInstance != null && stateInstance is IUiState uiState)
                            states.Add(uiState);
                    }
                    catch
                    {
                        //ignore
                    }
                }
            }
            container.ComposeParts(states);
        }
        return states.FirstOrDefault();
    }

    private static IEnumerable<Type> GetAllTypesThatImplementInterface<T>(string path)
    => AppDomain.CurrentDomain.GetAssemblies()
            .Where(x => x.Location.Contains(path, StringComparison.CurrentCultureIgnoreCase))
            .SelectMany(x => x.GetTypes())
            .Where(x => typeof(T).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract);
}