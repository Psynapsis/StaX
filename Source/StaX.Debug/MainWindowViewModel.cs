using ReactiveUI;
using StaX.Domain;

namespace StaX.Debug;

public class MainWindowViewModel : ViewModelBase
{
    private IUiState? _currentStateContent;
    public IUiState? CurrentStateContent
    {
        get => _currentStateContent;
        private set => this.RaiseAndSetIfChanged(ref _currentStateContent, value);
    }

    public MainWindowViewModel()
    {
    }

    public MainWindowViewModel(IUiState uiState)
    {
        uiState.StateView.DataContext = uiState.StateViewModel;
        CurrentStateContent = uiState;
        this.RaisePropertyChanging(nameof(CurrentStateContent));
    }
}