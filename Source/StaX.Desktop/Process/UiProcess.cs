using Avalonia.Threading;
using DynamicData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using StaX.Domain;

namespace StaX.Desktop.Process;

public class UiProcess
{
    private readonly Subject<UiTransition> _stateChangedSubject = new();

    private HomeState _homeState;

    public List<LazyUiState> AvailableStates { get; } = [];

    public IObservable<UiTransition> StateChanged => _stateChangedSubject;

    public async Task AddStatesAsync(IEnumerable<LazyUiState> uiStateTridderPairs)
    {
        await Dispatcher.UIThread.InvokeAsync(() => {
            _homeState = new HomeState(uiStateTridderPairs.ToList());
            _homeState.OnTransitionChanged.Subscribe(Transit);

            var lazyHome = new LazyUiState(_homeState);
            AvailableStates.Add(lazyHome);
            AvailableStates.Add(uiStateTridderPairs);

            foreach (var uiState in uiStateTridderPairs.Where(x => x is ITransientUiState || x is ICanMoveUiState))
            {
                if (uiState is ITransientUiState currentState)
                    currentState.OnTransitionChanged.Subscribe(Transit);
                if (uiState is ICanMoveUiState canMoveState)
                    canMoveState.OnMoveChanged.Subscribe(Transit);
            }
        });

        await Task.CompletedTask;
    }

    private void Transit(LazyUiState lazyUiState) => _stateChangedSubject.OnNext(new(lazyUiState?.UiState ?? _homeState));
    private void Transit(UiTransition uiTransition) => _stateChangedSubject.OnNext(uiTransition);
    private async void Transit(Transition transition)
    {
        try
        {
            var uiTransition = await GetFrom(AvailableStates, transition);
            if (uiTransition is not null)
                Transit(uiTransition);
        }
        catch
        {
            //ingnore
        }
    }

    public static async Task<UiTransition> GetFrom(List<LazyUiState> availableStates, Transition transition)
    {
        LazyUiState? uiState = availableStates.Where(x => x.StateName == transition.NameState).FirstOrDefault();
        if (uiState != null)
        {
            if (uiState.IsLoaded == false)
                await uiState.InitializeAsync();

            if (uiState.UiState is not null)
                return new UiTransition(uiState.UiState, transition.Parameter);
        }

        return new UiTransition(availableStates.FirstOrDefault(x => x.IsLoaded)?.UiState ?? new HomeState([]));
    }
}