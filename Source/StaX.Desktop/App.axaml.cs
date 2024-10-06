using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using StaX.Desktop.Models;
using StaX.Desktop.Views;
using System;
using System.Threading.Tasks;

namespace StaX.Desktop;

public partial class App : Application
{
    private readonly StartupArgs _args;

    public App(string[]? args)
    {
        _args = new(args);
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;
    }

    public App()
    {
        _args = StartupArgs.Empty;
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