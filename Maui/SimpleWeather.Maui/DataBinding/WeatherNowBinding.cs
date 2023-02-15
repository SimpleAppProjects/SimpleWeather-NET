using System;
using SimpleWeather.Common.ViewModels;
using SimpleWeather.LocationData;

namespace SimpleWeather.Maui.DataBinding
{
	public static class WeatherNowBinding
	{
        public static bool IsLoadingRingActive(WeatherNowState state)
        {
            return (state.IsLoading && (state.NoLocationAvailable || string.IsNullOrWhiteSpace(state.Weather?.Location))) || state.IsImageLoading;
        }
        public static bool IsViewVisible(WeatherNowState state)
        {
            return (state.IsLoading && (state.NoLocationAvailable || string.IsNullOrWhiteSpace(state.Weather?.Location))) ? false : true;
        }
        public static bool ShowGPSIcon(LocationType locationType)
        {
            return locationType == LocationType.GPS;
        }
        public static bool ShowGPSIcon(int locationType)
        {
            return locationType == (int)LocationType.GPS;
        }
    }
}

