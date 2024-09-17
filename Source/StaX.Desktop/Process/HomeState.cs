using FluentAvalonia.UI.Controls;
using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using StaX.Desktop.ViewModels;
using StaX.Desktop.Views;
using StaX.Domain;

namespace StaX.Desktop.Process;

public class HomeState : UiState, ITransientUiState
{
    public override string StateName { get; protected set; } = "Home";

    public override string ToolTip { get; protected set; } = "Go home";

    public override Symbol? Icon { get; protected set; } = Symbol.Home;

    public List<LazyUiState> States { get; }

    private Subject<Transition> _transitionSubject = new();
    public IObservable<Transition> OnTransitionChanged => _transitionSubject;

    public HomeState(List<LazyUiState> states)
    {
        States = states;
        StateView = new HomeView();
        StateViewModel = new HomeViewModel(States, _transitionSubject);
    }
}