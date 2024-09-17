using Avalonia.Collections;
using Avalonia.ReactiveUI;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Subjects;
using StaX.Desktop.Process;
using StaX.Domain;

namespace StaX.Desktop.ViewModels;

public class HomeViewModel : ViewModelBase
{
    private readonly Subject<Transition> _subject;

    private AvaloniaList<LazyUiState> _states;

    public AvaloniaList<LazyUiState> States
    {
        get => _states;
        set => this.RaiseAndSetIfChanged(ref _states, value);
    }

    public ReactiveCommand<string, Unit> SendSubjectCommand { get; }

    public HomeViewModel()
    {
        _subject = new Subject<Transition>();
        _states = [];
        SendSubjectCommand = ReactiveCommand.Create<string>(
            execute: SendSubject,
            outputScheduler: AvaloniaScheduler.Instance);
    }

    public HomeViewModel(List<LazyUiState> States, Subject<Transition> subject)
    {
        _states = new(States);
        _subject = subject;
        SendSubjectCommand = ReactiveCommand.Create<string>(
            execute: SendSubject,
            outputScheduler: AvaloniaScheduler.Instance);
    }

    public void SendSubject(string name) => _subject.OnNext(new(name));
}