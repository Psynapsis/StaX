using Avalonia;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using FluentAvalonia.UI.Windowing;
using System;
using StaX.Desktop.ViewModels;
using StaX.Desktop.Models;

namespace StaX.Desktop.Views;

public partial class MainWindow : AppWindow
{
    public MainWindow()
    {
        var mainWindowViewModel = new MainWindowViewModel();
        DataContext = mainWindowViewModel;
        AvaloniaXamlLoader.Load(this);

        SplashScreen = new MainAppSplashScreen(StartupArgs.Empty, mainWindowViewModel);
        TitleBar.ExtendsContentIntoTitleBar = true;
        TitleBar.TitleBarHitTestType = TitleBarHitTestType.Complex;

#if DEBUG
        this.AttachDevTools();
#endif
        if (Application.Current != null)
            Application.Current.ActualThemeVariantChanged += OnActualThemeVariantChanged;
    }

    public MainWindow(StartupArgs args)
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

        if (Application.Current != null)
            Application.Current.ActualThemeVariantChanged += OnActualThemeVariantChanged;
    }

    private void ThemeButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (Application.Current != null)
        {
            if (Application.Current.ActualThemeVariant == ThemeVariant.Light)
                Application.Current.RequestedThemeVariant = ThemeVariant.Dark;
            else
                Application.Current.RequestedThemeVariant = ThemeVariant.Light;
        }
    }

    private void OnActualThemeVariantChanged(object? sender, EventArgs e)
    {
        if (IsWindows11)
        {
            ClearValue(BackgroundProperty);
            ClearValue(TransparencyBackgroundFallbackProperty);
        }
    }
}