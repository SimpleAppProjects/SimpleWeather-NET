using SimpleWeather.Helpers;
using System.IO;

namespace SimpleWeather.Database
{
    public partial class LocationsDatabase
    {
        private static string GetDatabasePath()
        {
            var appDataFolderPath = ApplicationDataHelper.GetLocalFolderPath();
            return Path.Combine(appDataFolderPath, DatabaseFileName);
        }
    }
}
