using Avalonia;
using Avalonia.Controls;
using StaX.Desktop.Models;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading.Tasks;
using Path = System.IO.Path;

namespace StaX.Desktop.Process;

public class OutUiState : UserControl
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

    private readonly string _currentPluginFolder;

    private readonly Starter? _starter;

    public OutUiState(TopLevel topLevel, string currentPluginFolder)
    {
        _topLevel = topLevel;
        _starter = TryLoadStarter(currentPluginFolder);
        _currentPluginFolder = currentPluginFolder;

        _name = Path.Combine(currentPluginFolder, _starter?.Implementer);
        _path = _starter.Implementer;
    }

    private static Starter? TryLoadStarter(string path)
    {
        try
        {
            var json = File.ReadAllText(Path.Combine(path, "start.json"));
            return JsonSerializer.Deserialize<Starter>(json);
        }
        catch
        {
            //inore
        }
        return null;
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
            FileName = @"C:\Users\MegaD\StaX\RunnerBuild\StaXRunner.exe",
            Arguments = $"{_path}",
            UseShellExecute = false,
            RedirectStandardOutput = true,
        };
        if (processStartInfo is not null)
            _process = System.Diagnostics.Process.Start(processStartInfo);
    }

    private async Task ReceiveHwndAsync()
    {
        await Task.Run(() =>
        {
            using var server = new NamedPipeServerStream(_name, PipeDirection.In);
            server.WaitForConnection();
            using var reader = new BinaryReader(server);
            _childHwnd = (IntPtr)reader.ReadInt64();
        });
    }

    private void EmbedChildWindow()
    {
        try
        {
            IntPtr parentHwnd = _topLevel.TryGetPlatformHandle()!.Handle;

            _ = SetWindowLong(_childHwnd, GWL_STYLE, WS_VISIBLE | WS_CHILD);

            SetParent(_childHwnd, parentHwnd);

            var hostControl = this.FindControl<Control>("HostArea");
            hostControl?.GetObservable(BoundsProperty).Subscribe(bounds =>
            {
                SetWindowPos(
                    _childHwnd,
                    IntPtr.Zero,
                    (int)bounds.X,
                    (int)bounds.Y,
                    (int)(bounds.Width * 1.5),
                    (int)(bounds.Height * 1.5),
                    SWP_NOZORDER
                );
            });
        }
        catch (Exception)
        {
        }
    }
}