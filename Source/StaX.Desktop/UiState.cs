using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using FluentAvalonia.UI.Controls;
using Splat;
using System;
using System.IO;
using System.Reflection;

namespace StaX.Desktop;

public abstract partial class UiState : Application, IUiState
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private ViewModelBase _stateViewModel;
    private UserControl _stateView;

    public IReadonlyDependencyResolver DependencyResolver { get; private set; }

    public TopLevel TopLevel { get; private set; }

    static UiState() => UpdatePaths();

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public ViewModelBase StateViewModel
    {
        get => _stateViewModel;
        protected set => _stateViewModel = value;
    }

    public UserControl StateView
    {
        get => _stateView;
        protected set => _stateView = value;
    }

    public abstract string StateName { get; protected set; }

    public abstract string ToolTip { get; protected set; }

    public abstract Symbol? Icon { get; protected set; }

    /// <summary>
    /// Environment.CurrentDirectory
    /// </summary>
    public static string CurrentDirectory { get; private set; }

    /// <summary>
    /// Directory.GetCurrentDirectory()
    /// </summary>
    public static string DirectoryGetCurrent { get; private set; }

    /// <summary>
    /// AppDomain.CurrentDomain.BaseDirectory
    /// </summary>
    public static string AppDomainBaseDirectory { get; private set; }

    /// <summary>
    /// Executing assembly location (Assembly.GetExecutingAssembly().Location)
    /// </summary>
    public static string AssemblyLocation { get; private set; }

    /// <summary>
    /// Executing assembly directory (Path.GetDirectoryName(Assembly.GetExecutingAssembly()))
    /// </summary>
    public static string AssemblyDirectory { get; private set; }

    /// <summary>
    /// The Initialize method is triggered only on the first run, setting values for the ServiceProvider and TopLevel properties if they are not already initialized. If the properties already have values, the method does not change them.
    /// </summary>
    /// <param name="dependencyResolver"></param>
    /// <param name="topLevel"></param>
    public virtual void Initialize(IReadonlyDependencyResolver dependencyResolver, TopLevel topLevel)
    {
        DependencyResolver ??= dependencyResolver;
        TopLevel ??= topLevel;

        StateViewModel?.Initialize(
                DependencyResolver,
                TopLevel,
                new(CurrentDirectory,
                    DirectoryGetCurrent,
                    AppDomainBaseDirectory,
                    AssemblyLocation,
                    AssemblyDirectory));
    }

    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    private static void UpdatePaths()
    {
        CurrentDirectory = Environment.CurrentDirectory;
        DirectoryGetCurrent = Directory.GetCurrentDirectory();
        AppDomainBaseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        AssemblyLocation = Assembly.GetExecutingAssembly().Location;
        if (AssemblyLocation is not null)
        {
            var assemblyDirectory = Path.GetDirectoryName(AssemblyLocation);
            AssemblyDirectory = assemblyDirectory ?? string.Empty;
        }
    }
}