using CommunityToolkit.WinUI;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using SimpleWeather.NET.Helpers;
using System.Runtime.InteropServices;
using WinRT.Interop;
using WinUIEx;

namespace SimpleWeather.NET.Setup
{
    public partial class SetupPage
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
                titleBar.ButtonForegroundColor = Colors.White;
                titleBar.ButtonHoverBackgroundColor = this.FindResource("SimpleBlueLight") as Windows.UI.Color?;
                titleBar.ButtonHoverForegroundColor = Colors.WhiteSmoke;
                titleBar.ButtonPressedBackgroundColor = this.FindResource("SimpleBlueMedium") as Windows.UI.Color?;
                titleBar.ButtonPressedForegroundColor = Colors.WhiteSmoke;
                titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
                titleBar.ButtonInactiveForegroundColor = Colors.WhiteSmoke;
                titleBar.InactiveForegroundColor = Colors.WhiteSmoke;
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
                LeftPaddingColumn.Width = new GridLength(appWindow.TitleBar.LeftInset / scaleAdjustment);

                List<Windows.Graphics.RectInt32> dragRectsList = new();

                Windows.Graphics.RectInt32 dragRectL;
                var startPoint = AppTitleBar.GetScreenCoords();
                dragRectL.X = (int)(startPoint.X * scaleAdjustment);
#if DEBUG
                // DEBUG: Adjust for debug bar in Visual Studio
                dragRectL.Y = System.Diagnostics.Debugger.IsAttached ? 24 : 0;
#else
                dragRectL.Y = 0;
#endif
                dragRectL.Height = (int)((AppTitleBar.ActualHeight - dragRectL.Y) * scaleAdjustment);
                dragRectL.Width = (int)((LeftPaddingColumn.ActualWidth + TitleColumn.ActualWidth + LeftDragColumn.ActualWidth) * scaleAdjustment);
                dragRectsList.Add(dragRectL);

                Windows.Graphics.RectInt32 dragRectR;
                dragRectR.X = (int)((startPoint.X + LeftPaddingColumn.ActualWidth
                                    + TitleColumn.ActualWidth
                                    + LeftDragColumn.ActualWidth) * scaleAdjustment);
#if DEBUG
                // DEBUG: Adjust for debug bar in Visual Studio
                dragRectR.Y = System.Diagnostics.Debugger.IsAttached ? 24 : 0;
#else
                dragRectR.Y = 0;
#endif
                dragRectR.Height = (int)((AppTitleBar.ActualHeight - dragRectR.Y) * scaleAdjustment);
                dragRectR.Width = (int)(RightDragColumn.ActualWidth * scaleAdjustment);
                dragRectsList.Add(dragRectR);

                Windows.Graphics.RectInt32[] dragRects = dragRectsList.ToArray();

                appWindow.TitleBar.SetDragRectangles(dragRects);
            }
        }
    }
}
