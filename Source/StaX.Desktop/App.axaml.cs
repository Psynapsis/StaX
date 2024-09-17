using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.ReactiveUI;
using Avalonia.Markup.Xaml;
using System;
using System.IO;
using System.Threading.Tasks;
using StaX.Desktop.Views;

namespace StaX.Desktop;

public partial class App : Application
{
    private readonly string[]? _args = null;

    public App(string[]? args)
    {
        _args = args;
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;
    }

    public App()
    {
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            desktop.MainWindow = new MainWindow(_args);

        base.OnFrameworkInitializationCompleted();
    }

    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        => Console.WriteLine($"App stopped with exception", e);

    private void TaskSchedulerOnUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
        => Console.WriteLine($"Unobserved exception: {e.Exception.Message}{Environment.NewLine}{e.Exception.StackTrace}.");
}