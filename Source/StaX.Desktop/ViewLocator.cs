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
                var control = (uiState.UiState as OutUiState).StateView;
                return control;
            }
        }
        return null;
    }

    public bool Match(object? data) => data is LazyUiState;
} 