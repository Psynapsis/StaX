using Avalonia.ReactiveUI;
using Avalonia.Styling;
using Avalonia.Threading;
using ReactiveUI;
using Splat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using StaX.Desktop.Process;
using StaX.Domain;

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
        ChangeThemeCommand = ReactiveCommand.Create<string>(
            execute: ChangeTheme,
            outputScheduler: AvaloniaScheduler.Instance);
    }

    public void ChangeTheme(string v)
    {
        var application = Avalonia.Application.Current;
        if (application.ActualThemeVariant == ThemeVariant.Light)
            application.RequestedThemeVariant = ThemeVariant.Dark;
        else
            application.RequestedThemeVariant = ThemeVariant.Light;
    }

    private UiProcess uiProcess;

    public async Task StartProcessAsync()
    {
        uiProcess = Locator.Current.GetService<UiProcess>()!;
        var uiStateTridderPairs = Locator.Current.GetService<List<LazyUiState>>()!;

        if (uiProcess is not null && uiStateTridderPairs is not null)
        {
            await uiProcess.AddStatesAsync(uiStateTridderPairs);
            AvailableStates = new List<LazyUiState>(uiProcess.AvailableStates);
            uiProcess!.StateChanged.Subscribe(SetTransitionState);
        }

        if (AvailableStates?.Count == 2)
            SelectedState = AvailableStates.LastOrDefault();
        else if (AvailableStates?.Count > 0)
            SelectedState = AvailableStates.FirstOrDefault();

        if (AvailableStates is not null)
            foreach (var state in AvailableStates)
            {
                if (state.IsLoaded == false)
                    await state.InitializeAsync();

                if (state.UiState is UiState uiState)
                    uiState.Initialize(Locator.Current, TopLevelWidget.GetInstance());
            }
    }

    private void SetTransitionState(UiTransition selectedState)
    {
        var lazyState = uiProcess.AvailableStates.Where(x => x.StateName == selectedState.State.StateName).FirstOrDefault();
        if (lazyState is not null)
        {
            this.RaiseAndSetIfChanged(ref _selectedState, lazyState);
            if (selectedState.Parameter is not null)
                SetState(_selectedState, selectedState.Parameter);
            else
                SetState(_selectedState);
            this.RaisePropertyChanged(nameof(SelectedState));
        }
    }

    private void SetState(LazyUiState? selectedState)
    {
        if (selectedState is not null)
        {
            Dispatcher.UIThread.Invoke(async () =>
            {
                if (CurrentStateContent is not null)
                    await CurrentStateContent.UiState.StateViewModel.ExitActionAsync();

                CurrentStateContent = selectedState;

                if (CurrentStateContent is not null)
                    await CurrentStateContent.UiState.StateViewModel.EntryActionAsync();
            });
        }
    }

    private void SetState<TParameter>(LazyUiState? selectedState, TParameter parameter)
    {
        if (selectedState is not null)
        {
            Dispatcher.UIThread.Invoke(async () =>
            {
                if (CurrentStateContent is not null)
                    await CurrentStateContent.UiState.StateViewModel.ExitActionAsync();

                CurrentStateContent = selectedState;

                if (CurrentStateContent is not null)
                    await CurrentStateContent.UiState.StateViewModel.EntryActionAsync(parameter);
            });
        }
    }
}