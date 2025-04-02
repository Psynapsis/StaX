using Avalonia.Controls;
using Avalonia.Platform;
using StaX.Domain;

namespace StaX.Desktop.Process
{
    public class NativeHost : NativeControlHost
    {
        private INativeControl? _implementation;
        public INativeControl? Implementation
        {
            get => _implementation;
            set
            {
                _implementation = value;
                var platformHandle = GetTopLevel()?.TryGetPlatformHandle();
                if (platformHandle is not null)
                    CreateNativeControlCore(platformHandle);
            }
        }

        private static TopLevel? GetTopLevel() => TopLevelWidget.GetInstance();

        protected override IPlatformHandle CreateNativeControlCore(IPlatformHandle parent)
        {
            return Implementation?.CreateControl(parent)
                ?? base.CreateNativeControlCore(parent);
        }

        protected override void DestroyNativeControlCore(IPlatformHandle control)
        {
            base.DestroyNativeControlCore(control);
        }
    }

    public interface INativeControl
    {
        /// <param name="parent"></param>
        IPlatformHandle CreateControl(IPlatformHandle parent);
    }
}