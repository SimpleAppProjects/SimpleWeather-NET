#if __MACOS__ || MACCATALYST || IOS
using Foundation;
#endif
using System.IO;
#if WINUI
using Windows.Storage;
#else
using Microsoft.Maui.Storage;
#endif

namespace SimpleWeather.Helpers
{
    public static class ApplicationDataHelper
    {
        public static string GetLocalFolderPath()
        {
#if WINUI
            var appDataFolder = ApplicationData.Current.LocalFolder;
            var appDataFolderPath = appDataFolder.Path;
#else
            var appDataFolderPath = FileSystem.Current.AppDataDirectory;
#endif

            bool appendPath = false;
#if __MACOS__
            appendPath = NSProcessInfo.ProcessInfo.Environment["APP_SANDBOX_CONTAINER_ID"] != null; // Sandboxed
#elif WINDOWS
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

        public static string GetLocalCacheFolderPath()
        {
#if WINUI
            var appDataFolder = ApplicationData.Current.LocalCacheFolder;
            var appDataFolderPath = appDataFolder.Path;
#else
            var appDataFolderPath = FileSystem.Current.CacheDirectory;
#endif

            bool appendPath = false;
#if __MACOS__
            appendPath = NSProcessInfo.ProcessInfo.Environment["APP_SANDBOX_CONTAINER_ID"] != null; // Sandboxed
#elif WINDOWS
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

        public static string GetRootDataFolderPath()
        {
#if WINUI
            var appDataFolder = ApplicationData.Current.LocalFolder;
            var appDataFolderPath = appDataFolder.Path;
#else
            var appDataFolderPath = FileSystem.Current.AppDataDirectory;
#endif

            return Directory.GetParent(Path.TrimEndingDirectorySeparator(appDataFolderPath)).FullName;
        }
    }
}
