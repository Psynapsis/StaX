using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Platform;
using Avalonia.VisualTree;
using FluentAvalonia.UI.Controls;
using StaX.Desktop.Models;
using StaX.Domain;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Path = System.IO.Path;

namespace StaX.Desktop.Process;

public class OutUiState : TemplatedControl, IUiState, INativeControl
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
    private readonly string _name;
    private readonly TopLevel _topLevel;
    private System.Diagnostics.Process? _process;

    private readonly Starter? _starter;

    public ViewModelBase StateViewModel { get; set; } = new();

    public UserControl StateView { get; set; } = new();

    public string StateName => _starter?.Name ?? string.Empty;

    public string ToolTip => _starter?.ToolTip ?? string.Empty;

    public Symbol? Icon => _starter?.Symbol ?? Symbol.Add;

    public OutUiState(Starter starter, TopLevel topLevel, string currentPluginFolder)
    {
        _starter = starter;
        _topLevel = topLevel;

        _name = _starter?.Implementer ?? string.Empty;
        _path = Path.Combine(currentPluginFolder, "Plugin", _starter?.Implementer ?? string.Empty);
    }

    public async void Load()
    {
    }

    private void StartChildProcess()
    {
        var processStartInfo = new ProcessStartInfo
        {
            FileName = @"Runner\StaXRunner.exe",
            Arguments = $"{_path}",
            UseShellExecute = false,
            RedirectStandardOutput = true,
        };
        if (processStartInfo is not null)
            _process = System.Diagnostics.Process.Start(processStartInfo);
    }

    private async Task ReceiveHwndAsync()
    {
        using var server = new NamedPipeServerStream(_name, PipeDirection.In);
        server.WaitForConnection();
        using var reader = new BinaryReader(server);
        _childHwnd = (IntPtr)reader.ReadInt64();
    }

    private IDisposable _boundsSubscription;

    private void UpdateChildWindowPosition(Rect bounds)
    {
        // Получаем абсолютные координаты контрола относительно TopLevel
        var gridHost = _topLevel.GetControl<Grid>("ChildGridHost");
        var transform = gridHost.TransformToVisual(_topLevel);
        var positionInTopLevel = transform?.Transform(new Point(0, 0)) ?? new Point(0, 0);

        // Учитываем DPI scaling
        var scaling = _topLevel.GetVisualRoot()?.RenderScaling ?? 1;

        // Конвертируем в экранные координаты
        var screenPoint = _topLevel.PointToScreen(positionInTopLevel);

        int x = (int)(screenPoint.X * scaling);
        int y = (int)(screenPoint.Y * scaling);
        int width = (int)(gridHost.Bounds.Width * scaling);
        int height = (int)(gridHost.Bounds.Height * scaling);

        if (_childHwnd != IntPtr.Zero)
        {
            SetWindowPos(
                _childHwnd,
                IntPtr.Zero,
                x,
                y,
                width,
                height,
                SWP_NOZORDER
            );
        }
    }

    public IPlatformHandle CreateControl(IPlatformHandle parent)
    {
        StartChildProcess();
        ReceiveHwndAsync().GetAwaiter().GetResult();

        var kek = SetWindowLong(_childHwnd, GWL_STYLE, WS_VISIBLE | WS_CHILD);
        var ptr = SetParent(_childHwnd, parent.Handle);

        var gridHost = _topLevel.GetControl<Grid>("ChildGridHost");

        UpdateChildWindowPosition(gridHost.Bounds);
        _boundsSubscription = gridHost.GetObservable(BoundsProperty)
            .Subscribe(UpdateChildWindowPosition);

        return new PlatformHandle(_childHwnd, "HWND");
    }
}