using SimpleWeather.SkiaSharp;
using System;
using System.Threading.Tasks;

namespace SimpleWeather.Icons
{
    public partial interface IWeatherIconsProvider
    {
        bool IsFontIcon { get; }
    }
    public partial interface IWeatherIconsProvider
    {
        Uri GetWeatherIconURI(string icon);
        String GetWeatherIconURI(string icon, bool isAbsoluteUri, bool isLight = false);
        Task<SKDrawable> GetDrawable(string icon, bool isLight = false);
    }

    public interface ILottieWeatherIconProvider
    {
        public abstract String GetLottieIconURI(string icon);
    }

    public interface IXamlWeatherIconProvider
    {
        public abstract String GetXamlIconUri(string icon);
    }

    public interface ISVGWeatherIconProvider
    {
        public abstract String GetSVGIconUri(string icon, bool isLight = false);
    }
}
