using SimpleWeather.SkiaSharp;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
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
                    var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(lottieProvider.GetLottieIconURI(icon)));

                    drawable = Animation.Create((await file.OpenReadAsync()).AsStreamForRead())?.ToDrawable();
                }

                if (drawable == null)
                {
                    var svg = new Svg.Skia.SKSvg();

                    var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(GetSVGIconUri(icon, isLight)));

                    svg.Load((await file.OpenReadAsync()).AsStreamForRead());
                    drawable = svg.ToDrawable();
                }

                if (drawable == null)
                {
                    var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(GetWeatherIconURI(icon, isAbsoluteUri: true, isLight)));

                    drawable = SKBitmap.Decode((await file.OpenReadAsync()).AsStreamForRead()).ToDrawable();
                }

                return drawable;
            });
        }
    }
}
