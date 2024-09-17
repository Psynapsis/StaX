using Avalonia;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using FluentAvalonia.UI.Windowing;
using System;
using StaX.Desktop.ViewModels;

namespace StaX.Desktop.Views;

public partial class MainWindow : AppWindow
{
    public MainWindow()
    {
        var mainWindowViewModel = new MainWindowViewModel();
        DataContext = mainWindowViewModel;
        AvaloniaXamlLoader.Load(this);

        SplashScreen = new MainAppSplashScreen(null, mainWindowViewModel);
        TitleBar.ExtendsContentIntoTitleBar = true;
        TitleBar.TitleBarHitTestType = TitleBarHitTestType.Complex;

#if DEBUG
        this.AttachDevTools();
#endif

        Application.Current.ActualThemeVariantChanged += OnActualThemeVariantChanged;
    }

    public MainWindow(string[]? args)
    {
        var mainWindowViewModel = new MainWindowViewModel();
        DataContext = mainWindowViewModel;
        AvaloniaXamlLoader.Load(this);

        SplashScreen = new MainAppSplashScreen(args, mainWindowViewModel);
        TitleBar.ExtendsContentIntoTitleBar = true;
        TitleBar.TitleBarHitTestType = TitleBarHitTestType.Complex;

#if DEBUG
        this.AttachDevTools();
#endif

        Application.Current.ActualThemeVariantChanged += OnActualThemeVariantChanged;
    }

    private void ThemeButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var application = Application.Current;
        if (application.ActualThemeVariant == ThemeVariant.Light)
            application.RequestedThemeVariant = ThemeVariant.Dark;
        else
            application.RequestedThemeVariant = ThemeVariant.Light;
    }

    private void OnActualThemeVariantChanged(object sender, EventArgs e)
    {
        if (IsWindows11)
        {
            ClearValue(BackgroundProperty);
            ClearValue(TransparencyBackgroundFallbackProperty);
        }
    }
}