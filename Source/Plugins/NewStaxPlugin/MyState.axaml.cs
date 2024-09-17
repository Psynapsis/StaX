using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using FluentAvalonia.UI.Controls;
using NewStaxPlugin.ViewModels;
using NewStaxPlugin.Views;
using StaX.Domain;

namespace NewStaxPlugin
{
    public partial class MyState : UiState
    {
        public override string StateName { get; protected set; } = "NewStaxPlugin";
        public override string ToolTip { get; protected set; } = "NewStaxPlugin";
        public override Symbol? Icon { get; protected set; } = Symbol.ShareAndroid;

        public MyState()
        {
            StateView = new MyView();
            StateViewModel = new MyViewModel();
        }

        public override void Initialize() => AvaloniaXamlLoader.Load(this);
    }
}