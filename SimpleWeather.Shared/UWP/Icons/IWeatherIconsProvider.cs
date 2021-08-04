using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using muxc = Microsoft.UI.Xaml.Controls;

namespace SimpleWeather.Icons
{
    public partial interface IWeatherIconsProvider
    {
        Uri GetWeatherIconURI(string icon);
        String GetWeatherIconURI(string icon, bool isAbsoluteUri, bool isLight = false);
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

    public interface ISVGWeatherIconProvider
    {
        public abstract String GetSVGIconUri(string icon, bool isLight = false);
    }
}
