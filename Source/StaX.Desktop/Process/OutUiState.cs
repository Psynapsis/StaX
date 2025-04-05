using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform;
using StaX.Desktop.Models;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace StaX.Desktop.Process
{
    public class OutUiState : NativeControlHost
    {
        [DllImport("user32.dll")]
        private static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter,
            int X, int Y, int cx, int cy, uint uFlags);

        // Константы стилей
        private const int GWL_STYLE = -16;
        private const int WS_CHILD = 0x40000000;
        private const int WS_VISIBLE = 0x10000000;
        private const int WS_CAPTION = 0x00C00000;

        // Флаги для SetWindowPos
        private const uint SWP_NOZORDER = 0x0004;
        private const uint SWP_NOACTIVATE = 0x0010;
        private const uint SWP_FRAMECHANGED = 0x0020;
        private const int HWND_BOTTOM = 1;

        private IntPtr _childHwnd;
        private readonly string _path;
        private readonly string _name;
        private System.Diagnostics.Process? _process;
        private readonly Starter? _starter;

        public OutUiState(Starter starter, string currentPluginFolder)
        {
            _starter = starter;
            _name = _starter?.Implementer ?? string.Empty;
            _path = Path.Combine(currentPluginFolder, "Plugin", _starter?.Implementer ?? string.Empty);
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
            _process = System.Diagnostics.Process.Start(processStartInfo);
        }

        private async Task ReceiveHwndAsync()
        {
            using var server = new NamedPipeServerStream(_name, PipeDirection.In);
            server.WaitForConnection();
            using var reader = new BinaryReader(server);
            var current = reader.ReadInt64();
            _childHwnd = (IntPtr)current;
        }

        protected override IPlatformHandle CreateNativeControlCore(IPlatformHandle parent)
        {
            StartChildProcess();
            ReceiveHwndAsync().GetAwaiter().GetResult();

            // Убираем рамку и заголовок окна
            var style = GetWindowLong(_childHwnd, GWL_STYLE);
            SetWindowLong(_childHwnd, GWL_STYLE, (style & ~WS_CAPTION) | WS_CHILD | WS_VISIBLE);

            // Устанавливаем родителя
            SetParent(_childHwnd, parent.Handle);

            // Позиционируем внизу Z-порядка
            SetWindowPos(
                _childHwnd,
                (IntPtr)HWND_BOTTOM,
                0,
                0,
                (int)Bounds.Width,
                (int)Bounds.Height,
                SWP_FRAMECHANGED | SWP_NOACTIVATE
            );

            return new PlatformHandle(_childHwnd, "HWND");
        }

        protected override void DestroyNativeControlCore(IPlatformHandle control)
        {
            _process?.Kill();
            base.DestroyNativeControlCore(control);
        }
    }
}