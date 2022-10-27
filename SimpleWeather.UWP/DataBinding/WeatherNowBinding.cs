using Windows.UI.Xaml;

namespace SimpleWeather.UWP.DataBinding
{
    public static class WeatherNowBinding
    {
        public static bool IsLoadingRingActive(bool isLoading, bool noLocationAvailable, string locationName)
        {
            return (isLoading && (noLocationAvailable || string.IsNullOrWhiteSpace(locationName))) || isLoading;
        }
        public static Visibility IsViewVisible(bool isLoading, bool noLocationAvailable, string locationName)
        {
            return !IsLoadingRingActive(isLoading, noLocationAvailable, locationName) ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
