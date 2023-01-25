#if ANDROID

using Microsoft.Maui.Handlers;

namespace SimpleToolkit.Core.Handlers
{
    public partial class PopoverHandler : ElementHandler<IPopover, SimplePopupWindow>
    {
        protected override SimplePopupWindow CreatePlatformElement()
        {
            return new SimplePopupWindow(MauiContext.Context, MauiContext);
        }

        protected override void ConnectHandler(SimplePopupWindow platformView)
        {
            platformView.SetElement(VirtualView);
            platformView.DismissEvent += PlatformView_DismissEvent;
            base.ConnectHandler(platformView);
        }

        protected override void DisconnectHandler(SimplePopupWindow platformView)
        {
            platformView.DismissEvent -= PlatformView_DismissEvent;
            platformView.CleanUp();
        }

        private void PlatformView_DismissEvent(object sender, EventArgs e)
        {
            if (VirtualView is Popover popover)
            {
                popover.OnClosed();
            }
        }

        public static void MapContent(PopoverHandler handler, IPopover popover)
        {
            handler.PlatformView.SetContent(popover);
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