#if __MACOS__
using Foundation;
#endif
using System.IO;
using Windows.Storage;

namespace SimpleWeather.Helpers
{
    public static class ApplicationDataHelper
    {
        public static string GetLocalFolderPath()
        {
            var appDataFolder = ApplicationData.Current.LocalFolder;
            var appDataFolderPath = appDataFolder.Path;

            bool appendPath = false;
#if __MACOS__
            appendPath = NSProcessInfo.ProcessInfo.Environment["APP_SANDBOX_CONTAINER_ID"] != null; // Sandboxed
#elif WINDOWS_UWP
            // appendPath = false;
#elif __IOS__ || __SKIA__ || NETSTANDARD2_0
            appendPath = true;
#endif

            if (appendPath)
            {
                appDataFolderPath = Path.Combine(appDataFolderPath, "SimpleWeather");
            }

            Directory.CreateDirectory(appDataFolderPath);

            return appDataFolderPath;
        }
    }
}
