using Microsoft.UI;
using Microsoft.UI.Xaml;
using SimpleWeather.Utils;
using System;
using System.Linq;
using System.Reflection;
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

        // Source: https://github.com/microsoft/microsoft-ui-xaml/issues/7782#issuecomment-1266997865

        /// <summary>
        /// Set the Icon for this <see cref="Window"/> from an *.ico from the filesystem
        /// </summary>
        /// <param name="window"></param>
        /// <param name="iconPath">The <see cref="Path"/> to the *.ico file</param>
        public static void SetIconFromFilesystem(this Window window, string iconPath)
        {
            try
            {
                var appWindow = window.GetAppWindow();
                appWindow.SetIcon(iconPath);
            }
            catch (Exception e)
            {
                Logger.WriteLine(LoggerLevel.Error, e, "unhandled exception when set icon for window");
            }
        }

        /// <summary>
        /// Set the Icon for this <see cref="Window"/> out from the current process, which is the same as the ApplicationIcon set in the *.csproj
        /// </summary>
        /// <param name="window"></param>
        public static void SetIconFromApplicationIcon(this Window window)
        {
            try
            {
                // https://learn.microsoft.com/en-us/answers/questions/822928/app-icon-windows-app-sdk.html
                string sExe = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
                var ico = System.Drawing.Icon.ExtractAssociatedIcon(sExe);
                window.SetIcon(Win32Interop.GetIconIdFromIcon(ico.Handle));
            }
            catch (Exception e)
            {
                Logger.WriteLine(LoggerLevel.Error, e, "unhandled exception when set icon for window");
            }
        }

        /// <summary>
        /// Set the Icon for this <see cref="Window"/> out from an EmbeddedResource. If no <see cref="Assembly"/> is specified, the current loaded <see cref="Assembly"/> is used for
        /// </summary>
        /// <param name="window"></param>
        /// <param name="resourceName">The name of the resource</param>
        /// <param name="assembly">Location of the resource</param>
        public static void SetIconFromEmbeddedResource(this Window window, string resourceName, Assembly assembly = null)
        {
            try
            {
                // https://github.com/microsoft/microsoft-ui-xaml/issues/7782#issuecomment-1266928339
                if (assembly == null) assembly = Assembly.GetEntryAssembly();

                var rName = assembly.GetManifestResourceNames().FirstOrDefault(s => s.EndsWith(resourceName, StringComparison.InvariantCultureIgnoreCase));
                var icon = new System.Drawing.Icon(assembly.GetManifestResourceStream(rName));

                var appWindow = window.GetAppWindow();
                appWindow.SetIcon(Win32Interop.GetIconIdFromIcon(icon.Handle));
            }
            catch (Exception e)
            {
                Logger.WriteLine(LoggerLevel.Error, e, "unhandled exception when set icon for window");
            }
        }
    }
}
