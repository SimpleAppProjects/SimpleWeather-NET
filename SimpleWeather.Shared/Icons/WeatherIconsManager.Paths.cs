using SimpleWeather.SkiaSharp;
using System;
using System.IO;
using System.Threading.Tasks;
#if WINUI
using Windows.Storage;
#else
using Microsoft.Maui.Storage;
#endif
using SKBitmap = SkiaSharp.SKBitmap;

namespace SimpleWeather.Icons
{
    public sealed partial class WeatherIconsManager : IWeatherIconsProvider
    {
        public static string GetPNGBaseUri(bool isLight = false)
        {
#if WINUI
            return "ms-appx:///SimpleWeather.Shared/Resources/Images/WeatherIcons/png/" + (isLight ? "light/" : "dark/");
#else
            return "SimpleWeather.Shared/Images/WeatherIcons/png/" + (isLight ? "light/" : "dark/");
#endif
        }

        public static string GetSVGBaseUri(bool isLight = false)
        {
#if WINUI
            return "ms-appx:///SimpleWeather.Shared/Resources/Images/WeatherIcons/svg/" + (isLight ? "light/" : "dark/");
#else
            return "SimpleWeather.Shared/Images/WeatherIcons/svg/" + (isLight ? "light/" : "dark/");
#endif
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
#if WINUI
            var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(GetWeatherIconURI(icon, isAbsoluteUri: true, isLight)));
            var fStream = (await file.OpenReadAsync()).AsStreamForRead();
#else
            var fStream = await FileSystem.OpenAppPackageFileAsync(GetWeatherIconURI(icon, isAbsoluteUri: true, isLight));
#endif

            return SKBitmap.Decode(fStream).ToDrawable();
        }
    }
}
