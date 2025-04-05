using Avalonia.ReactiveUI;
using Avalonia.Styling;
using Avalonia.Threading;
using ReactiveUI;
using Splat;
using StaX.Desktop.Process;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;

namespace StaX.Desktop.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private LazyUiState? _currentStateContent;
    private LazyUiState? _selectedState;

    private List<LazyUiState>? _availableStates;

    public List<LazyUiState>? AvailableStates
    {
        get => _availableStates?.Where(x => x is not ISilentUiState).ToList();
        private set => this.RaiseAndSetIfChanged(ref _availableStates, value);
    }

    public LazyUiState? SelectedState
    {
        get => _selectedState;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedState, value);
            SetState(_selectedState);
        }
    }

    public LazyUiState? CurrentStateContent
    {
        get => _currentStateContent;
        private set => this.RaiseAndSetIfChanged(ref _currentStateContent, value);
    }

    public ReactiveCommand<string, Unit> ChangeThemeCommand { get; }

    public MainWindowViewModel()
    {
        _uiProcess = new();
        ChangeThemeCommand = ReactiveCommand.Create<string>(
            execute: ChangeTheme,
            outputScheduler: AvaloniaScheduler.Instance);
    }

    public static void ChangeTheme(string v)
    {
        if (Avalonia.Application.Current != null)
        {
            var application = Avalonia.Application.Current;
            if (application.ActualThemeVariant == ThemeVariant.Light)
                application.RequestedThemeVariant = ThemeVariant.Dark;
            else
                application.RequestedThemeVariant = ThemeVariant.Light;
        }
    }

    private UiProcess _uiProcess;

    public async Task StartProcessAsync()
    {
        _uiProcess = Locator.Current.GetService<UiProcess>()!;
        var uiStateTridderPairs = Locator.Current.GetService<List<LazyUiState>>()!;

        if (_uiProcess is not null && uiStateTridderPairs is not null)
        {
            await _uiProcess.AddStatesAsync(uiStateTridderPairs);
            AvailableStates = [.. _uiProcess.AvailableStates];
            _uiProcess!.StateChanged.Subscribe(SetTransitionState);
        }

        if (AvailableStates?.Count == 2)
            SetState(AvailableStates.LastOrDefault());
        else if (AvailableStates?.Count > 0)
            SetState(AvailableStates.FirstOrDefault());
    }

    private void SetTransitionState(UiTransition selectedState)
    {
        var lazyState = _uiProcess.AvailableStates.Where(x => x.StateName == selectedState.State.StateName).FirstOrDefault();
        if (lazyState is not null)
        {
            if (selectedState.Parameter is not null)
                SetState(lazyState, selectedState.Parameter);
            else
                SetState(lazyState);
        }
    }

    private void SetState(LazyUiState? selectedState)
    {
        if (selectedState is not null)
        {
            this.RaiseAndSetIfChanged(ref _selectedState, selectedState);
            Dispatcher.UIThread.Invoke(async () =>
            {
                await LoadSelectedStateAsync(selectedState);

                CurrentStateContent = selectedState;
            });
            this.RaisePropertyChanged(nameof(SelectedState));
            this.RaisePropertyChanged(nameof(CurrentStateContent));
        }
    }

    private void SetState<TParameter>(LazyUiState? selectedState, TParameter parameter)
    {
        if (selectedState is not null)
        {
            this.RaiseAndSetIfChanged(ref _selectedState, selectedState);
            Dispatcher.UIThread.Invoke(async () =>
            {
                await LoadSelectedStateAsync(selectedState);

                CurrentStateContent = selectedState;
            });
            this.RaisePropertyChanged(nameof(SelectedState));
            this.RaisePropertyChanged(nameof(CurrentStateContent));
        }
    }

    private static async Task LoadSelectedStateAsync(LazyUiState selectedState)
    {
        if (selectedState.IsLoaded == false)
            await selectedState.InitializeAsync();
    }
}