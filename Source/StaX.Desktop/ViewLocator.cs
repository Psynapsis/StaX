using Avalonia.Controls;
using Avalonia.Controls.Templates;
using StaX.Desktop.Process;

namespace StaX.Desktop.Desktop;

public class ViewLocator : IDataTemplate
{
    public Control? Build(object? data)
    {
        if (data is not null && data is LazyUiState uiState)
        {
            if (uiState.UiState is not null)
            {
                var control = uiState.UiState.StateView;
                control.DataContext = uiState.UiState.StateViewModel;
                return control;
            }
        }
        return null;
    }

    public bool Match(object? data) => data is LazyUiState;
}