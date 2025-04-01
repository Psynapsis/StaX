using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

namespace StaX.RunnerHost;

public partial class App : Application
{
    string _uiState;
    public App(string[] args)
    {
        _uiState = args[0];
    }

    public App()
    {
    }

    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    public override void OnFrameworkInitializationCompleted()
    {
        base.OnFrameworkInitializationCompleted();

        if (_uiState is not null)
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                desktop.MainWindow = new MainWindow(_uiState);

            EntryAction();
        }
    }

    private static async void EntryAction()
    {

    }
}