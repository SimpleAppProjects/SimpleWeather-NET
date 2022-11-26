﻿using System.IO;
using Windows.Storage;

namespace SimpleWeather.Database
{
    public partial class TZDatabase
    {
        private static string GetDatabasePath()
        {
            var appDataFolder = ApplicationData.Current.LocalFolder;
            return Path.Combine(appDataFolder.Path, DatabaseFileName);
        }
    }
}
