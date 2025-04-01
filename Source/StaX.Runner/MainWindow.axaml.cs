using Avalonia;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using FluentAvalonia.UI.Windowing;
using Splat;
using StaX.Domain;
using System.IO.Pipes;
using System.IO;
using System;
using System.Threading.Tasks;
using AsyncAwaitBestPractices;

namespace StaX.Runner;

public partial class MainWindow : AppWindow
{
    public MainWindow()
    {
        var mainWindowViewModel = new MainWindowViewModel();
        DataContext = mainWindowViewModel;
        AvaloniaXamlLoader.Load(this);

        TitleBar.ExtendsContentIntoTitleBar = true;
        TitleBar.TitleBarHitTestType = TitleBarHitTestType.Complex;
        this.Opened += (s, e) => SendHwndToParent();
        this.AttachDevTools();
    }

    private void SendHwndToParent(string host = "HwndPipe")
    {
        Task.Run(() =>
        {
            try
            {
                IntPtr hwnd = TryGetPlatformHandle()!.Handle;
                using var client = new NamedPipeClientStream(".", host, PipeDirection.Out);
                client.Connect();
                using var writer = new BinaryWriter(client);
                writer.Write(hwnd.ToInt64());
            }
            catch (Exception)
            {
            }
        }).SafeFireAndForget();
    }

    public MainWindow(IUiState uiState)
    {
        var mainWindowViewModel = new MainWindowViewModel(uiState);
        DataContext = mainWindowViewModel;
        AvaloniaXamlLoader.Load(this);

        TitleBar.ExtendsContentIntoTitleBar = true;
        TitleBar.TitleBarHitTestType = TitleBarHitTestType.Complex;
        this.Opened += (s, e) => SendHwndToParent();

        this.AttachDevTools();
    }
}