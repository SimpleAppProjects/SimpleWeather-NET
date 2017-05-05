using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWeather
{
    public interface WeatherLoadedListener
    {
        void onWeatherLoaded(int locationIdx, Object weather);
    }
}
