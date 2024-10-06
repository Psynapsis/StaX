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
    private readonly Subject<LazyUiState> _subject;

    private AvaloniaList<LazyUiState> _states;

    public AvaloniaList<LazyUiState> States
    {
        get => _states;
        set => this.RaiseAndSetIfChanged(ref _states, value);
    }

    public ReactiveCommand<LazyUiState, Unit> SendSubjectCommand { get; }

    public HomeViewModel()
    {
        _subject = new Subject<LazyUiState>();
        _states = [];
        SendSubjectCommand = ReactiveCommand.Create<LazyUiState>(
            execute: SendSubject,
            outputScheduler: AvaloniaScheduler.Instance);
    }

    public HomeViewModel(List<LazyUiState> States, Subject<LazyUiState> subject)
    {
        _states = new(States);
        _subject = subject;
        SendSubjectCommand = ReactiveCommand.Create<LazyUiState>(
            execute: SendSubject,
            outputScheduler: AvaloniaScheduler.Instance);
    }

    public void SendSubject(LazyUiState state) => _subject.OnNext(state);
}