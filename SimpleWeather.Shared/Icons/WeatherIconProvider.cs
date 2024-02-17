using SimpleWeather.SkiaSharp;
using System;
using System.IO;
using System.Threading.Tasks;
#if WINUI
using Windows.Storage;
#else
using Microsoft.Maui.Storage;
#endif
using Animation = SkiaSharp.Skottie.Animation;
using SKBitmap = SkiaSharp.SKBitmap;
using SimpleWeather.Utils;

namespace SimpleWeather.Icons
{
    public abstract partial class WeatherIconProvider : IWeatherIconsProvider, ISVGWeatherIconProvider
    {
        public abstract string Key { get; }
        public abstract string DisplayName { get; }
        public abstract string AuthorName { get; }
        public abstract Uri AttributionLink { get; }
        public abstract bool IsFontIcon { get; }

        public abstract Uri GetWeatherIconURI(string icon);
        public abstract String GetWeatherIconURI(string icon, bool isAbsoluteUri, bool isLight = false);
        public abstract String GetSVGIconUri(string icon, bool isLight = false);

        public virtual Task<SKDrawable> GetDrawable(string icon, bool isLight = false)
        {
            return Task.Run(async () =>
            {
                SKDrawable drawable = null;

                if (this is ILottieWeatherIconProvider lottieProvider)
                {
#if WINUI
                    var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(lottieProvider.GetLottieIconURI(icon, isLight)));
                    var fStream = (await file.OpenReadAsync()).AsStreamForRead();
#elif __ANDROID__
                    var filename = Path.GetFileName(lottieProvider.GetLottieIconURI(icon, isLight));
                    var extension = (isLight ? "light" : "dark") + Path.GetExtension(filename);
                    var themeFile = $"{Path.GetFileNameWithoutExtension(filename)}_{extension}";
                    var fStream = await FileSystemUtils.OpenAppPackageFileAsync(themeFile);
#else
                    var fStream = await FileSystemUtils.OpenAppPackageFileAsync(lottieProvider.GetLottieIconURI(icon, isLight));
#endif

                    drawable = Animation.Create(fStream)?.ToDrawable();
                }

                if (drawable == null)
                {
                    drawable = await GetSVGDrawable(icon, isLight);
                }

                if (drawable == null)
                {
                    drawable = await GetBitmapDrawable(icon, isLight);
                }

                return drawable;
            });
        }

        public virtual Task<SKDrawable> GetSVGDrawable(string icon, bool isLight = false)
        {
            return Task.Run(async () =>
            {
                var svg = new Svg.Skia.SKSvg();

#if WINUI
                var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(GetSVGIconUri(icon, isLight)));
                var fStream = (await file.OpenReadAsync()).AsStreamForRead();
#elif __ANDROID__
                var filename = Path.GetFileName(GetSVGIconUri(icon, isLight));
                var extension = (isLight ? "light" : "dark") + Path.GetExtension(filename);
                var themeFile = $"{Path.GetFileNameWithoutExtension(filename)}_{extension}";
                var fStream = await FileSystemUtils.OpenAppPackageFileAsync(themeFile);
#else
                var fStream = await FileSystemUtils.OpenAppPackageFileAsync(GetSVGIconUri(icon, isLight));
#endif

                svg.Load(fStream);
                return svg.ToDrawable();
            });
        }

        public virtual Task<SKDrawable> GetBitmapDrawable(string icon, bool isLight = false)
        {
            return Task.Run(async () =>
            {
#if WINUI
                var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(GetWeatherIconURI(icon, isAbsoluteUri: true, isLight)));
                var fStream = (await file.OpenReadAsync()).AsStreamForRead();
#elif __ANDROID__
                var filename = Path.GetFileName(GetWeatherIconURI(icon, isAbsoluteUri: false, isLight));
                var extension = (isLight ? "light" : "dark") + Path.GetExtension(filename);
                var themeFile = $"{Path.GetFileNameWithoutExtension(filename)}_{extension}";
                var fStream = await FileSystemUtils.OpenAppPackageFileAsync(themeFile);
#else
                var fStream = await FileSystemUtils.OpenAppPackageFileAsync(GetWeatherIconURI(icon, isAbsoluteUri: true, isLight));
#endif

                return SKBitmap.Decode(fStream).ToDrawable();
            });
        }
    }
}
