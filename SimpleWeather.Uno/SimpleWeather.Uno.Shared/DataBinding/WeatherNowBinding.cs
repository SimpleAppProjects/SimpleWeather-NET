using SimpleWeather.Common.ViewModels;
using SimpleWeather.LocationData;
using Microsoft.UI.Xaml;

namespace SimpleWeather.Uno.DataBinding
{
    public static class WeatherNowBinding
    {
        public static bool IsLoadingRingActive(WeatherNowState state)
        {
            return (state.IsLoading && (state.NoLocationAvailable || string.IsNullOrWhiteSpace(state.Weather?.Location))) || state.IsImageLoading;
        }
        public static Visibility IsViewVisible(WeatherNowState state)
        {
            return (state.IsLoading && (state.NoLocationAvailable || string.IsNullOrWhiteSpace(state.Weather?.Location))) ? Visibility.Collapsed : Visibility.Visible;
        }
        public static Visibility ShowGPSIcon(LocationType locationType)
        {
            return locationType == LocationType.GPS ? Visibility.Visible : Visibility.Collapsed;
        }
        public static Visibility ShowGPSIcon(int locationType)
        {
            return locationType == (int)LocationType.GPS ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
