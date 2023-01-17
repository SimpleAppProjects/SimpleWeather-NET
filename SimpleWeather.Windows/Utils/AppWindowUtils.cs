using Microsoft.UI.Xaml;
using Windows.Foundation;
using WinUIEx;

namespace SimpleWeather.NET.Utils
{
    public static class AppWindowUtils
    {
        public static void SetMinSize(this Window window, Size size)
        {
            var windowMgr = WindowManager.Get(window);
            windowMgr.MinHeight = size.Height;
            windowMgr.MinWidth = size.Width;
        }
    }
}
