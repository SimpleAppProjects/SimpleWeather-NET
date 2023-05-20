using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Windows.Widgets.Providers;
using SimpleWeather.Common.WeatherData;
using SimpleWeather.Preferences;
using SimpleWeather.WeatherData;
using System.Collections.Immutable;

namespace SimpleWeather.NET.Widgets.Templates
{
    public abstract class AbstractWidgetCreator
    {
        protected readonly SettingsManager SettingsManager = Ioc.Default.GetService<SettingsManager>();

        public abstract Task<string> BuildUpdate(string widgetId, WidgetInfo info);

        public virtual Task<string> BuildWidgetData(string widgetId, WidgetInfo info)
        {
            return Task.FromResult("${}");
        }

        protected Task<LocationData.LocationData> GetLocation(string widgetId)
        {
            return Task.Run(async () =>
            {
                if (WidgetUtils.IsGPS(widgetId))
                {
                    if (!SettingsManager.FollowGPS)
                    {
                        await WidgetUpdateHelper.ResetGPSWidgets(ImmutableList.Create(widgetId));
                        return null;
                    }
                    else
                    {
                        return await SettingsManager.GetLastGPSLocData();
                    }
                }
                else
                {
                    return WidgetUtils.GetLocationData(widgetId);
                }
            });
        }

        protected async Task<Weather> LoadWeather(LocationData.LocationData locData)
        {
            try
            {
                // If saved data DNE (for current location), refresh weather
                var wLoader = new WeatherDataLoader(locData);
                var weather = await wLoader.LoadWeatherData(
                    new WeatherRequest.Builder()
                        .ForceLoadSavedData()
                        .Build()
                );

                if (weather == null)
                {
                    weather = await wLoader.LoadWeatherData(
                        new WeatherRequest.Builder()
                            .ForceRefresh(false)
                            .LoadAlerts()
                            .LoadForecasts()
                            .Build()
                    );
                }

                return weather;
            }
            catch
            {
                return null;
            }
        }
    }
}
