using Avalonia;
using Avalonia.ReactiveUI;

namespace NewStaxPlugin
{
    internal class Plugin
    {
        /*
            Initialization code. Don't use any Avalonia, third-party APIs or any
            SynchronizationContext-reliant code before AppMain is called: things aren't initialized
            yet and stuff might break.
            Avalonia configuration, don't remove; used by visual designer.
        */

        [STAThread]
        public static void Main(string[] args) => BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);

        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<MyState>()
                .UsePlatformDetect()
                .WithInterFont()
                .LogToTrace()
                .UseReactiveUI();
    }
}