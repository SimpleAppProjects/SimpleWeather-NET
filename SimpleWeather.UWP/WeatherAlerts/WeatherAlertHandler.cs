﻿using CommunityToolkit.Mvvm.DependencyInjection;
using SimpleWeather.Preferences;
using SimpleWeather.UWP.Notifications;
using SimpleWeather.Weather_API;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleWeather.UWP.WeatherAlerts
{
    public static class WeatherAlertHandler
    {
        public static async Task PostAlerts(LocationData.LocationData location, ICollection<WeatherAlert> alerts)
        {
            var wm = WeatherModule.Instance.WeatherManager;

            // Post weather alert notifications
            if (wm.SupportsAlerts && alerts != null && alerts.Any())
            {
                // Only alert if we're in the background
#if DEBUG
                if (true)
#else
                if (App.Current.AppState == AppState.Background)
#endif
                {
                    // Check if any of these alerts have been posted before
                    // or are past the expiration date
#if DEBUG
                    var unotifiedAlerts = alerts;
#else
                    var now = DateTimeOffset.Now;
                    var unotifiedAlerts = alerts.Where(alert => alert.Notified == false && alert.ExpiresDate > now && alert.Date <= now);
#endif

                    // Post any un-notified alerts
                    await WeatherAlertCreator.CreateAlerts(location, unotifiedAlerts);

                    // Update all alerts
                    await SetasNotified(location, alerts);
                }
            }
        }

        public static async Task SetasNotified(LocationData.LocationData location, ICollection<WeatherAlert> alerts)
        {
            if (alerts != null)
            {
                var now = DateTimeOffset.Now;

                // Update all alerts
                foreach (var alert in alerts)
                {
                    if (alert.Date <= now)
                    {
                        alert.Notified = true;
                    }
                }

                // Save alert data
                var SettingsManager = Ioc.Default.GetService<SettingsManager>();
                await SettingsManager.SaveWeatherAlerts(location, alerts);
            }
        }
    }
}