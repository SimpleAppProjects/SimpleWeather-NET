using SimpleWeather.SkiaSharp;
using System;
using System.IO;
using System.Threading.Tasks;
#if WINUI
using Windows.Storage;
#else
using Microsoft.Maui.Storage;
#endif

namespace SimpleWeather.Icons
{
    public sealed partial class WeatherIconsManager : IWeatherIconsProvider
    {
        public static string PathToThemeFile(string filePath, bool isLight = false)
        {
            var filename = Path.GetFileName(filePath);
            var extension = (isLight ? "light" : "dark") + Path.GetExtension(filename);
            var themeFile = $"{Path.GetFileNameWithoutExtension(filename)}_{extension}";

            return themeFile;
        }

        public string GetPNGBaseUri(bool isLight = false)
        {
            return _IconsProvider.GetPNGBaseUri(isLight);
        }

        public string GetSVGBaseUri(bool isLight = false)
        {
            return _IconsProvider.GetSVGBaseUri(isLight);
        }

        public Uri GetWeatherIconURI(string icon)
        {
            return _IconsProvider.GetWeatherIconURI(icon);
        }

        public string GetWeatherIconURI(string icon, bool isAbsoluteUri, bool isLight = false)
        {
            return _IconsProvider.GetWeatherIconURI(icon, isAbsoluteUri, isLight);
        }

        public Task<SKDrawable> GetDrawable(string icon, bool isLight = false)
        {
            return _IconsProvider.GetDrawable(icon, isLight);
        }

        public Task<SKDrawable> GetBitmapDrawable(string icon, bool isLight = false)
        {
            return _IconsProvider.GetBitmapDrawable(icon, isLight);
        }
    }
}
