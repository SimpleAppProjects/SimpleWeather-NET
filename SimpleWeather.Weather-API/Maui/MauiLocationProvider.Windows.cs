#if WINDOWS
using SimpleWeather.LocationData;
using SimpleWeather.Utils;
using SimpleWeather.Weather_API.Bing;
using SimpleWeather.Weather_API.Utils;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace SimpleWeather.Weather_API.Maui
{
    public partial class MauiLocationProvider : BingMapsLocationProvider { }
}
#endif