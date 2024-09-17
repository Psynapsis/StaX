using Avalonia;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using FluentAvalonia.UI.Windowing;
using Splat;
using StaX.Domain;

namespace StaX.Debug;

public partial class MainWindow : AppWindow
{
    public MainWindow()
    {
        var mainWindowViewModel = new MainWindowViewModel();
        DataContext = mainWindowViewModel;
        AvaloniaXamlLoader.Load(this);

        TitleBar.ExtendsContentIntoTitleBar = true;
        TitleBar.TitleBarHitTestType = TitleBarHitTestType.Complex;

        this.AttachDevTools();
    }

    public MainWindow(IUiState uiState)
    {
        var mainWindowViewModel = new MainWindowViewModel(uiState);
        DataContext = mainWindowViewModel;
        AvaloniaXamlLoader.Load(this);

        TitleBar.ExtendsContentIntoTitleBar = true;
        TitleBar.TitleBarHitTestType = TitleBarHitTestType.Complex;

        this.AttachDevTools();
    }
}