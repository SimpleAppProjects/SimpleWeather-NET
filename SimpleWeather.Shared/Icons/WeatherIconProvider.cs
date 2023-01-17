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
                    var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(lottieProvider.GetLottieIconURI(icon)));
                    var fStream = (await file.OpenReadAsync()).AsStreamForRead();
#else
                    var fStream = await FileSystem.OpenAppPackageFileAsync(lottieProvider.GetLottieIconURI(icon));
#endif

                    drawable = Animation.Create(fStream)?.ToDrawable();
                }

                if (drawable == null)
                {
                    var svg = new Svg.Skia.SKSvg();

#if WINUI
                    var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(GetSVGIconUri(icon, isLight)));
                    var fStream = (await file.OpenReadAsync()).AsStreamForRead();
#else
                    var fStream = await FileSystem.OpenAppPackageFileAsync(GetSVGIconUri(icon, isLight));
#endif

                    svg.Load(fStream);
                    drawable = svg.ToDrawable();
                }

                if (drawable == null)
                {
#if WINUI
                    var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(GetWeatherIconURI(icon, isAbsoluteUri: true, isLight)));
                    var fStream = (await file.OpenReadAsync()).AsStreamForRead();
#else
                    var fStream = await FileSystem.OpenAppPackageFileAsync(GetWeatherIconURI(icon, isAbsoluteUri: true, isLight));
#endif

                    drawable = SKBitmap.Decode(fStream).ToDrawable();
                }

                return drawable;
            });
        }
    }
}
