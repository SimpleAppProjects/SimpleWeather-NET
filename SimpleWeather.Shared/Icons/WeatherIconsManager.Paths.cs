using SimpleWeather.SkiaSharp;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using SKBitmap = SkiaSharp.SKBitmap;

namespace SimpleWeather.Icons
{
    public sealed partial class WeatherIconsManager : IWeatherIconsProvider
    {
        public static string GetPNGBaseUri(bool isLight = false)
        {
            return "ms-appx:///SimpleWeather.Shared/Assets/WeatherIcons/png/" + (isLight ? "light/" : "dark/");
        }

        public static string GetSVGBaseUri(bool isLight = false)
        {
            return "ms-appx:///SimpleWeather.Shared/Assets/WeatherIcons/svg/" + (isLight ? "light/" : "dark/");
        }

        public Uri GetWeatherIconURI(String icon)
        {
            return _IconsProvider.GetWeatherIconURI(icon);
        }

        public String GetWeatherIconURI(String icon, bool isAbsoluteUri, bool isLight = false)
        {
            return _IconsProvider.GetWeatherIconURI(icon, isAbsoluteUri, isLight);
        }

        public async Task<SKDrawable> GetDrawable(string icon, bool isLight = false)
        {
            var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(GetWeatherIconURI(icon, isAbsoluteUri: true, isLight)));

            return SKBitmap.Decode((await file.OpenReadAsync()).AsStreamForRead()).ToDrawable();
        }
    }
}
