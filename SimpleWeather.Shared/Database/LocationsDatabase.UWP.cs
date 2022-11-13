#if WINDOWS_UWP
using System.IO;
using Windows.Storage;

namespace SimpleWeather.Database
{
    public partial class LocationsDatabase
    {
        private static string GetDatabasePath()
        {
            var appDataFolder = ApplicationData.Current.LocalFolder;
            return Path.Combine(appDataFolder.Path, DatabaseFileName);
        }
    }
}
#endif