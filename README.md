
# StaX

StaX is a cross-platform application built on Avalonia and Fluent Avalonia, designed to load and display plugins with various functionalities. The application utilizes the `StaX.Domain` NuGet library, allowing you to easily extend its capabilities by developing your own plugins.

## Key Features

- **Plugin Loading**: You can add plugins to the application by simply placing them in the `Plugins` folder. StaX will automatically detect and display them.
- **Flexibility**: Plugins are implemented as descendants of the `UiState` class from the `StaX.Domain` library. This allows you to develop plugins with any functionality and seamlessly integrate them into the main application.
- **Modern UI**: The app leverages Avalonia and Fluent Avalonia, providing a modern and user-friendly interface.

## Screenshots

_Main window of the application:_

![Main Window](https://imgur.com/a/mdXp74u)

## Installation and Usage

### Requirements
To run StaX, you will need:
- .NET 6 or higher
- [StaX.Domain NuGet package](https://www.nuget.org/packages/StaX.Domain)

### Building

1. Clone the repository:
    ```bash
    git clone https://github.com/Psynapsis/StaX.git
    ```

2. Navigate to the project directory and restore dependencies:
    ```bash
    cd StaX
    dotnet restore
    ```

3. Build and run the application:
    ```bash
    dotnet build
    dotnet run
    ```

### Adding Plugins

To add a plugin to StaX, follow these steps:

1. Create a new project or library that uses the `StaX.Domain` package.
2. Implement your plugin by creating a class that inherits from `StaX.Domain.UiState`.
3. Build the plugin as a DLL.
4. Place the compiled plugin in the `Plugins` folder next to the StaX main application.
5. StaX will automatically load and display the plugin the next time it runs.

## Example Plugin

Here’s an example of a simple plugin:

```csharp
using Avalonia.Markup.Xaml;
using FluentAvalonia.UI.Controls;
using MyStaxPlugin.ViewModels;
using MyStaxPlugin.Views;
using StaX.Domain;

namespace MyStaxPlugin;

public partial class MyState : UiState
{
    public override string StateName { get; protected set; } = "MyStaxPlugin";
    public override string ToolTip { get; protected set; } = "MyStaxPlugin";
    public override Symbol? Icon { get; protected set; } = Symbol.ShareAndroid;

    public MyState()
    {
        StateView = new MyView();
        StateViewModel = new MyViewModel();
    }

    public override void Initialize() => AvaloniaXamlLoader.Load(this);
}
```

## Contributing

If you'd like to contribute to the development of StaX or create plugins for it, feel free to fork the repository and submit your changes via a Pull Request.

## License

This project is licensed under the MIT License. For more details, see the [LICENSE](./LICENSE) file.
