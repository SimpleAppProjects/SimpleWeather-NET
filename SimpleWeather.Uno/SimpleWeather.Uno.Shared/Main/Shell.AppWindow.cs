#if WINDOWS
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using SimpleWeather.Uno.Utils;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using WinRT.Interop;

namespace SimpleWeather.Uno.Main
{
    public partial class Shell
    {
        private AppWindow m_AppWindow;

        private void SetupAppTitleBar()
        {
            if (AppWindowTitleBar.IsCustomizationSupported())
            {
                m_AppWindow = MainWindow.Current.GetAppWindow();
                var titleBar = m_AppWindow.TitleBar;
                titleBar.ExtendsContentIntoTitleBar = true;
                AppTitleBar.Loaded += AppTitleBar_Loaded;
                AppTitleBar.SizeChanged += AppTitleBar_SizeChanged;
            }
            else
            {
                MainWindow.Current.ExtendsContentIntoTitleBar = true;
                MainWindow.Current.SetTitleBar(AppTitleBar);
            }
        }

        private void UpdateTitleBarTheme()
        {
            if (AppWindowTitleBar.IsCustomizationSupported() && m_AppWindow != null)
            {
                var titleBar = m_AppWindow.TitleBar;
                titleBar.ButtonBackgroundColor = Colors.Transparent;
                titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
                titleBar.ButtonForegroundColor = Colors.White;
                titleBar.InactiveForegroundColor = Colors.WhiteSmoke;
                titleBar.ButtonInactiveForegroundColor = Colors.WhiteSmoke;
            }
        }

        private void AppTitleBar_Loaded(object sender, RoutedEventArgs e)
        {
            if (AppWindowTitleBar.IsCustomizationSupported())
            {
                SetDragRegionForCustomTitleBar(m_AppWindow);
            }
        }

        private void AppTitleBar_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (AppWindowTitleBar.IsCustomizationSupported()
                && m_AppWindow.TitleBar.ExtendsContentIntoTitleBar)
            {
                // Update drag region if the size of the title bar changes.
                SetDragRegionForCustomTitleBar(m_AppWindow);
            }
        }

        [DllImport("Shcore.dll", SetLastError = true)]
        internal static extern int GetDpiForMonitor(IntPtr hmonitor, Monitor_DPI_Type dpiType, out uint dpiX, out uint dpiY);

        internal enum Monitor_DPI_Type : int
        {
            MDT_Effective_DPI = 0,
            MDT_Angular_DPI = 1,
            MDT_Raw_DPI = 2,
            MDT_Default = MDT_Effective_DPI
        }

        private double GetScaleAdjustment()
        {
            IntPtr hWnd = WindowNative.GetWindowHandle(MainWindow.Current);
            WindowId wndId = Win32Interop.GetWindowIdFromWindow(hWnd);
            DisplayArea displayArea = DisplayArea.GetFromWindowId(wndId, DisplayAreaFallback.Primary);
            IntPtr hMonitor = Win32Interop.GetMonitorFromDisplayId(displayArea.DisplayId);

            // Get DPI.
            int result = GetDpiForMonitor(hMonitor, Monitor_DPI_Type.MDT_Default, out uint dpiX, out uint _);
            if (result != 0)
            {
                throw new Exception("Could not get DPI for monitor.");
            }

            uint scaleFactorPercent = (uint)(((long)dpiX * 100 + (96 >> 1)) / 96);
            return scaleFactorPercent / 100.0;
        }

        private void SetDragRegionForCustomTitleBar(AppWindow appWindow)
        {
            if (AppWindowTitleBar.IsCustomizationSupported()
                && appWindow.TitleBar.ExtendsContentIntoTitleBar)
            {
                double scaleAdjustment = GetScaleAdjustment();

                RightPaddingColumn.Width = new GridLength(appWindow.TitleBar.RightInset / scaleAdjustment);
                //LeftPaddingColumn.Width = new GridLength(appWindow.TitleBar.LeftInset / scaleAdjustment);

                List<Windows.Graphics.RectInt32> dragRectsList = new();

                Windows.Graphics.RectInt32 dragRectL;
                dragRectL.X = (int)((LeftPaddingColumn.ActualWidth) * scaleAdjustment);
                dragRectL.Y = 0;
                dragRectL.Height = (int)(AppTitleBar.ActualHeight * scaleAdjustment);
                dragRectL.Width = (int)((TitleColumn.ActualWidth + LeftDragColumn.ActualWidth) * scaleAdjustment);
                dragRectsList.Add(dragRectL);

                Windows.Graphics.RectInt32 dragRectR;
                dragRectR.X = (int)((LeftPaddingColumn.ActualWidth
                                    + TitleColumn.ActualWidth
                                    + LeftDragColumn.ActualWidth) * scaleAdjustment);
                dragRectR.Y = 0;
                dragRectR.Height = (int)(AppTitleBar.ActualHeight * scaleAdjustment);
                dragRectR.Width = (int)(RightDragColumn.ActualWidth * scaleAdjustment);
                dragRectsList.Add(dragRectR);

                Windows.Graphics.RectInt32[] dragRects = dragRectsList.ToArray();

                appWindow.TitleBar.SetDragRectangles(dragRects);
            }
        }
    }
}
#endif