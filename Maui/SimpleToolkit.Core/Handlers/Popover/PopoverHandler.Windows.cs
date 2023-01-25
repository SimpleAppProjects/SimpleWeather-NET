#if WINDOWS

using Microsoft.Maui.Handlers;

// Partially based on the .NET MAUI Community Toolkit Popup control - https://github.com/CommunityToolkit/Maui

namespace SimpleToolkit.Core.Handlers
{
    public partial class PopoverHandler : ElementHandler<IPopover, SimpleFlyout>
    {
        protected override SimpleFlyout CreatePlatformElement()
        {
            return new SimpleFlyout(MauiContext);
        }

        protected override void ConnectHandler(SimpleFlyout platformView)
        {
            platformView.SetElement(VirtualView);
            platformView.Opened += PlatformView_Opened;
            platformView.Closed += PlatformView_Closed;
            base.ConnectHandler(platformView);
        }

        protected override void DisconnectHandler(SimpleFlyout platformView)
        {
            platformView.Opened -= PlatformView_Opened;
            platformView.Closed -= PlatformView_Closed;
            platformView.CleanUp();
        }

        private void PlatformView_Opened(object sender, object e)
        {
            if (VirtualView is Popover popover)
            {
                popover.OnOpened();
            }
        }

        private void PlatformView_Closed(object sender, object e)
        {
            if (VirtualView is Popover popover)
            {
                popover.OnClosed();
            }
        }

        public static void MapContent(PopoverHandler handler, IPopover popover)
        {
            handler.PlatformView.ConfigureControl();
        }

        public static void MapShow(PopoverHandler handler, IPopover popover, object parentView)
        {
            if (parentView is not IElement anchor)
                return;

            handler.PlatformView.Show(anchor);
        }

        public static void MapHide(PopoverHandler handler, IPopover popover, object arg3)
        {
            handler.PlatformView.Hide();
        }
    }
}

#endif