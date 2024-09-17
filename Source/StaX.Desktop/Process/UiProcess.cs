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

    public List<LazyUiState> AvailableStates { get; } = [];

    public IObservable<UiTransition> StateChanged => _stateChangedSubject;

    public async Task AddStatesAsync(IEnumerable<LazyUiState> uiStateTridderPairs)
    {
        await Dispatcher.UIThread.InvokeAsync(() => {
            var home = new HomeState(uiStateTridderPairs.ToList());
            home.OnTransitionChanged.Subscribe(Transit);

            var lazyHome = new LazyUiState(home);
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

    private void Transit(UiTransition uiTransition) => _stateChangedSubject.OnNext(uiTransition);
    private async void Transit(Transition transition)
    {
        try
        {
            var uiTransition = await GetFrom(AvailableStates, transition);
            if (uiTransition is not null)
                Transit(uiTransition);
        }
        catch (Exception ex)
        {
            //ingnore
        }
    }

    public static async Task<UiTransition> GetFrom(List<LazyUiState> availableStates, Transition transition)
    {
        LazyUiState uiState = availableStates.Where(x => x.StateName == transition.NameState).FirstOrDefault();
        if (uiState != null)
        {
            if (uiState.IsLoaded == false)
                await uiState.InitializeAsync();
            return new UiTransition(uiState.UiState, transition.Parameter);
        }

        return new UiTransition(availableStates.FirstOrDefault(x => x.IsLoaded).UiState);
    }
}