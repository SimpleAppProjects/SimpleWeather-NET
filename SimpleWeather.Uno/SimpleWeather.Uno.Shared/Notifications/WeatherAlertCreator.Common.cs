#if !WINDOWS
using SimpleWeather.Common.Controls;
using SimpleWeather.Icons;
using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Notifications;

namespace SimpleWeather.Uno.Notifications
{
    public static class WeatherAlertCreator
    {
        private const string TAG = "WeatherAlerts";

        public static async Task CreateAlerts(LocationData.LocationData location, IEnumerable<WeatherAlert> alerts)
        {
            foreach (WeatherAlert alert in alerts)
            {
                if (alert.Date > DateTimeOffset.Now)
                    continue;

                var alertVM = new WeatherAlertViewModel(alert);
            }
        }
    }
}
#endif