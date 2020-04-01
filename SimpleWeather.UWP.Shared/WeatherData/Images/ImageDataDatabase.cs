using SimpleWeather.WeatherData.Images;
using SimpleWeather.WeatherData.Images.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace SimpleWeather.WeatherData.Images
{
    public sealed partial class ImageDatabaseSnapshotCacheImpl
    {
        internal static String imageDBPath =
            Path.Combine(ApplicationData.Current.LocalCacheFolder.Path, "imageStore");
    }
}
