using Avalonia.Controls;
using FluentAvalonia.UI.Controls;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StaX.Desktop;

public interface IUiState<out TStateVM, out TStateV>
    where TStateVM : ViewModelBase
    where TStateV : UserControl
{
    TStateVM StateViewModel { get; }
    TStateV StateView { get; }

    string StateName { get; }
    string ToolTip { get; }
    Symbol? Icon { get; }
}

public record Transition(string NameState, object? Parameter = null);

public record UiTransition(IUiState State, object? Parameter = null)
{
    public static UiTransition GetFrom(List<IUiState> availableStates, Transition transition)
    {
        var state = availableStates.Where(x => x.StateName == transition.NameState).FirstOrDefault();
        if (state is not null)
            return new(state, transition.Parameter);
        else
            return new(availableStates.FirstOrDefault()!, null);
    }
}

public interface IUiState : IUiState<ViewModelBase, UserControl>
{
}

public interface ISilentUiState : IUiState
{
}

public interface ITransientUiState : IUiState
{
    IObservable<Transition> OnTransitionChanged { get; }
}

public interface ICanMoveUiState : IUiState
{
    IObservable<UiTransition> OnMoveChanged { get; }
}