using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using muxc = Microsoft.UI.Xaml.Controls;

namespace SimpleWeather.Icons
{
    public abstract partial class WeatherIconProvider : IWeatherIconsProvider
    {
        public abstract Uri GetWeatherIconURI(string icon);
        public abstract String GetWeatherIconURI(string icon, bool isAbsoluteUri, bool isLight = false);
    }

    public interface ILottieWeatherIconProvider
    {
        public abstract String GetLottieIconURI(string icon);
    }

    public interface ILottieGenWeatherIconProvider
    {
        public abstract muxc.IAnimatedVisualSource GetLottieGenSource(string icon);
    }

    public interface IXamlWeatherIconProvider
    {
        public abstract String GetXamlIconUri(string icon);
    }
}
