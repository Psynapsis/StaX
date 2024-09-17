using Avalonia.Collections;
using NewStaxPlugin.Models;
using ReactiveUI;
using StaX.Domain;

namespace NewStaxPlugin.ViewModels
{
    public class MyViewModel : ViewModelBase
    {
        private AvaloniaList<StyledLog> _logs = [];
        public AvaloniaList<StyledLog> Logs
        {
            get => _logs;
            set => this.RaiseAndSetIfChanged(ref _logs, value);
        }

        private bool _isVisible;
        public bool IsVisible
        {
            get => _isVisible;
            set => this.RaiseAndSetIfChanged(ref _isVisible, value);
        }

        public override Task EntryActionAsync()
        {
            Logs.Add(new("CurrentDirectory: " + CurrentDirectory, LogType.Error));
            Logs.Add(new("DirectoryGetCurrent: " + DirectoryGetCurrent, LogType.Warning));
            Logs.Add(new("AppDomainBaseDirectory: " + AppDomainBaseDirectory, LogType.Warning));
            Logs.Add(new("AssemblyLocation: " + AssemblyLocation, LogType.Info));
            Logs.Add(new("AssemblyDirectory: " + AssemblyDirectory, LogType.Info));
            return base.EntryActionAsync();
        }

        public Task Something()
        {
            Logs.Add(new("Something", LogType.Warning));
            return Task.CompletedTask;
        }
    }
}