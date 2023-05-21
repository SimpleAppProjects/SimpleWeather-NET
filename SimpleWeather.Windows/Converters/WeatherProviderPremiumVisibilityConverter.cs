using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using SimpleWeather.Extras;

namespace SimpleWeather.NET.Converters
{
    public class WeatherProviderPremiumVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var api = value?.ToString();

            var ExtrasService = Ioc.Default.GetService<IExtrasService>();

            return (api != null && ExtrasService.IsPremiumWeatherAPI(api)) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}