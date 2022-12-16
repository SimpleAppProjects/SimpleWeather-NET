using SimpleWeather.Helpers;
using System.IO;

namespace SimpleWeather.Database
{
    public partial class TZDatabase
    {
        private static string GetDatabasePath()
        {
            var appDataFolderPath = ApplicationDataHelper.GetLocalFolderPath();
            return Path.Combine(appDataFolderPath, DatabaseFileName);
        }
    }
}
