using System.Collections.Generic;

namespace StaX.Desktop.Models;

public class StartupArgs(string[]? _args)
{
    public static readonly StartupArgs Empty = new(null);

    public List<string> Args = _args is not null ? new(_args) : [];

    public bool NotEmpty => _args is not null && _args.Length > 0;
}