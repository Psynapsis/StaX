using DynamicData;
using FluentAvalonia.UI.Controls;
using Microsoft.Extensions.DependencyInjection;
using Splat.Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using StaX.Desktop.Process;
using StaX.Desktop.Views;
using Avalonia.Threading;

namespace StaX.Desktop;

internal class ServiceLocator
{
    private const string PluginsDir = "Plugins";
    private PluginLoaderVisualizator _pluginLoaderVisualizator;

    public Task RegisterAsync(PluginLoaderVisualizator pluginLoaderVisualizator, string[]? args)
    {
        _pluginLoaderVisualizator = pluginLoaderVisualizator;
        var serviceCollection = new ServiceCollection();

        LoadPlugins(serviceCollection, args);
        serviceCollection.UseMicrosoftDependencyResolver();

        return Task.CompletedTask;
    }

    private void LoadPlugins(IServiceCollection services, string[]? args)
    {
        try
        {
            List<LazyUiState> states = [];
            List<string> statesStrings = [];

            if (args is not null)
                foreach (var unknownString in args)
                    if (unknownString.Contains(".stx", StringComparison.CurrentCultureIgnoreCase))
                        statesStrings.Add(unknownString);

            if (statesStrings.Count > 0)
                states.AddRange(LoadXtoolPlugins(statesStrings));

            if (states.Count < 1)
            {
                var pluginsFullDir = Path.Combine(AppContext.BaseDirectory, PluginsDir);
                var pluginsDirectories = Directory.GetDirectories(pluginsFullDir);

                foreach (var directory in pluginsDirectories)
                    states.Add(GetLazy(directory));

                var files = Directory.GetFiles(pluginsFullDir);
                var stx = files.Where(f => !string.IsNullOrEmpty(f) && f.Contains(".stx", StringComparison.OrdinalIgnoreCase));
                states.Add(LoadXtoolPlugins(stx));
            }

            services.AddSingleton(states);
            services.AddSingleton<UiProcess>();
        }
        catch (Exception ex)
        {
            //
        }
    }

    private IEnumerable<LazyUiState> LoadXtoolPlugins(IEnumerable<string> pathXtools)
    {
        var states = new List<LazyUiState>();
        try
        {
            string tempDirectory = Path.Combine(Path.GetTempPath(), "StxFiles");

            if (!Directory.Exists(tempDirectory))
                Directory.CreateDirectory(tempDirectory);

            foreach (var tool in pathXtools)
            {
                string destinationFolder = Path.Combine(tempDirectory, Path.GetFileNameWithoutExtension(tool));

                if (!Directory.Exists(destinationFolder))
                    Directory.CreateDirectory(destinationFolder);

                string[] filesInStx = UnpackStx(tool, destinationFolder);

                var directory = Path.GetDirectoryName(filesInStx[0]);
                if (directory is not null)
                    states.Add(GetLazy(directory));
            }
        }
        catch (Exception ex)
        {
            //
        }
        return states;
    }

    private static string[] UnpackStx(string stxFilePath, string destinationFolder)
    {
        using (var zip = ZipFile.OpenRead(stxFilePath))
        {
            string[] extractedFiles = new string[zip.Entries.Count];

            foreach (var entry in zip.Entries)
            {
                string extractedFilePath = Path.Combine(destinationFolder, entry.FullName);

                Directory.CreateDirectory(Path.GetDirectoryName(extractedFilePath));

                entry.ExtractToFile(extractedFilePath, true);
                extractedFiles[Array.IndexOf(extractedFiles, null)] = extractedFilePath;
            }

            return extractedFiles;
        }
    }

    private void IconsAddThisState(LazyUiState uiState)
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            var collection = _pluginLoaderVisualizator.Icons;
            collection?.Add(uiState?.Icon ?? Symbol.Add);
            _pluginLoaderVisualizator.Icons = collection;
        });
    }

    private LazyUiState GetLazy(string path)
    {
        var lazyState = new LazyUiState(path);
        IconsAddThisState(lazyState);
        return lazyState;
    }
}