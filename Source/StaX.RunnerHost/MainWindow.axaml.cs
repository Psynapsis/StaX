using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using FluentAvalonia.UI.Windowing;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace StaX.RunnerHost;

public partial class MainWindow : AppWindow
{
    [DllImport("user32.dll")]
    private static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

    [DllImport("user32.dll")]
    private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

    [DllImport("user32.dll")]
    private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

    private const int GWL_STYLE = -16;
    private const int WS_CHILD = 0x40000000;
    private const int WS_VISIBLE = 0x10000000;
    private const uint SWP_NOZORDER = 0x0004;

    private IntPtr _childHwnd;

    private readonly string _path;
    private Process? _process;
    public MainWindow(string path)
    {
        _path = path;
        InitializeComponent();
        Loaded += OnLoaded;

        var mainWindowViewModel = new MainWindowViewModel();
        DataContext = mainWindowViewModel;

        TitleBar.ExtendsContentIntoTitleBar = true;
        TitleBar.TitleBarHitTestType = TitleBarHitTestType.Complex;

        this.AttachDevTools();
        AvaloniaXamlLoader.Load(this);
    }

    private async void OnLoaded(object? sender, EventArgs e)
    {
        StartChildProcess();
        await ReceiveHwndAsync();
        EmbedChildWindow();
    }

    private void StartChildProcess()
    {
        var processStartInfo = new ProcessStartInfo
        {
            FileName = @"C:\Users\konopelko-shumkovski\StaX\RunnerBuild\StaXRunner.exe",
            Arguments = $"{_path}",
            UseShellExecute = false,
            RedirectStandardOutput = true,
        };
        if (processStartInfo is not null)
            _process = Process.Start(processStartInfo);
    }

    private async Task ReceiveHwndAsync()
    {
        await Task.Run(() =>
        {
            using var server = new NamedPipeServerStream("HwndPipe", PipeDirection.In);
            server.WaitForConnection();
            using var reader = new BinaryReader(server);
            _childHwnd = (IntPtr)reader.ReadInt64();
        });
    }

    private void EmbedChildWindow()
    {
        try
        {
            IntPtr parentHwnd = TryGetPlatformHandle()!.Handle;

            _ = SetWindowLong(_childHwnd, GWL_STYLE, WS_VISIBLE | WS_CHILD);

            SetParent(_childHwnd, parentHwnd);

            var hostControl = this.FindControl<Control>("HostArea");
            hostControl?.GetObservable(BoundsProperty).Subscribe(bounds =>
            {
                SetWindowPos(
                    _childHwnd,
                    IntPtr.Zero,
                    (int)bounds.X,
                    (int)bounds.Y + 32,
                    (int)(bounds.Width),
                    (int)(bounds.Height - 32),
                    SWP_NOZORDER
                );
            });
        }
        catch (Exception)
        {
        }
    }
}