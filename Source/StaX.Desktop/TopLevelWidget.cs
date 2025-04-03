using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.VisualTree;
using System;

namespace StaX.Desktop;

public static class TopLevelWidget
{
    public static TopLevel GetInstance()
    {
        if (Application.Current != null)
            switch (Application.Current.ApplicationLifetime)
            {
                case ISingleViewApplicationLifetime singleViewPlatform:
                    {
                        var visualRoot = singleViewPlatform.MainView?.GetVisualRoot();
                        if (visualRoot is TopLevel root)
                            return root;

                        break;
                    }
                case IClassicDesktopStyleApplicationLifetime classicViewPlatform:
                    {
                        var visualRoot = classicViewPlatform.MainWindow?.GetVisualRoot();
                        if (visualRoot is TopLevel root)
                            return root;

                        break;
                    }
            }

        throw new Exception("Root view not found");
    }
}

public static class TopLevelExtensions
{
    public static void ForceSetFocus(this TopLevel topLevel)
    {
        var isFocusable = topLevel.Focusable;

        topLevel.Focusable = true;
        topLevel.Focus();
        topLevel.Focusable = isFocusable;
    }
}