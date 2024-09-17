using Avalonia;
using Avalonia.ReactiveUI;
using System;

namespace StaX.Debug;

internal class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args) => (args is null ? BuildAvaloniaApp() : BuildAvaloniaApp(args))
            .StartWithClassicDesktopLifetime(args ?? []);

    public static AppBuilder BuildAvaloniaApp(string[] args)
        => AppBuilder.Configure(() => new App(args))
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace()
            .UseReactiveUI();

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp() =>
        AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace()
            .UseReactiveUI();
}