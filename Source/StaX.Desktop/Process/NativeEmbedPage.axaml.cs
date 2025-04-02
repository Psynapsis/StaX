using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace StaX.Desktop.Process
{
    public partial class NativeEmbedPage : UserControl
    {
        public NativeEmbedPage()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);
        }
    }
}
