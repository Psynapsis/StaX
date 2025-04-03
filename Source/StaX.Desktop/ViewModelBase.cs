using Avalonia.Controls;
using ReactiveUI.Validation.Helpers;
using Splat;
using System.Threading.Tasks;

namespace StaX.Desktop;

public record Paths(string CurrentDirectory, string DirectoryGetCurrent, string AppDomainBaseDirectory, string AssemblyLocation, string AssemblyDirectory);

public class ViewModelBase : ReactiveValidationObject
{
    /// <summary>
    /// Environment.CurrentDirectory
    /// </summary>
    public static string? CurrentDirectory { get; private set; }

    /// <summary>
    /// Directory.GetCurrentDirectory()
    /// </summary>
    public static string? DirectoryGetCurrent { get; private set; }

    /// <summary>
    /// AppDomain.CurrentDomain.BaseDirectory
    /// </summary>
    public static string? AppDomainBaseDirectory { get; private set; }

    /// <summary>
    /// Executing assembly location (Assembly.GetExecutingAssembly().Location)
    /// </summary>
    public static string? AssemblyLocation { get; private set; }

    /// <summary>
    /// Executing assembly directory (Path.GetDirectoryName(Assembly.GetExecutingAssembly()))
    /// </summary>
    public static string? AssemblyDirectory { get; private set; }

    public IReadonlyDependencyResolver? DependencyResolver { get; private set; }

    public TopLevel? TopLevel { get; private set; }

    public virtual Task EntryActionAsync() => Task.CompletedTask;
    public virtual Task EntryActionAsync<TParameter>(TParameter parameter) => Task.CompletedTask;
    public virtual Task ExitActionAsync() => Task.CompletedTask;

    /// <summary>
    /// The Initialize method is triggered only on the first run, setting values for the ServiceProvider and TopLevel properties if they are not already initialized. If the properties already have values, the method does not change them.
    /// </summary>
    /// <param name="dependencyResolver"></param>
    /// <param name="topLevel"></param>
    public virtual void Initialize(IReadonlyDependencyResolver dependencyResolver, TopLevel topLevel, Paths paths)
    {
        DependencyResolver ??= dependencyResolver;
        TopLevel ??= topLevel;
        CurrentDirectory ??= paths.CurrentDirectory;
        DirectoryGetCurrent ??= paths.DirectoryGetCurrent;
        AppDomainBaseDirectory ??= paths.AppDomainBaseDirectory;
        AssemblyLocation ??= paths.AssemblyLocation;
        AssemblyDirectory ??= paths.AssemblyDirectory;
    }
}