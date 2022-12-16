using SimpleWeather.Helpers;
using System.IO;

namespace SimpleWeather.Database
{
    public partial class WeatherDatabase
    {
        private static string GetDatabasePath()
        {
            var appDataFolderPath = ApplicationDataHelper.GetLocalFolderPath();
            return Path.Combine(appDataFolderPath, DatabaseFileName);
        }
    }
}
