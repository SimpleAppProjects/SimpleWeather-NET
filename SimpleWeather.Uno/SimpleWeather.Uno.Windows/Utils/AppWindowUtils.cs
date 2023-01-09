using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using System;
using WinRT.Interop;

namespace SimpleWeather.Uno.Utils
{
    public static class AppWindowUtils
    {
        public static AppWindow GetAppWindow(this Window window)
        {
            IntPtr hWnd = WindowNative.GetWindowHandle(window);
            WindowId wndId = Win32Interop.GetWindowIdFromWindow(hWnd);
            return AppWindow.GetFromWindowId(wndId);
        }
    }
}
