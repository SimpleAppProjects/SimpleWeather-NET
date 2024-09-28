using Microsoft.UI.Xaml.Data;
using SimpleWeather.Common.Controls;

namespace SimpleWeather.NET.Converters
{
    public class DetailMapConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is IDictionary<WeatherDetailsType, DetailItemViewModel> map && map.TryGetValue((WeatherDetailsType)parameter, out DetailItemViewModel detail))
            {
                return $"{detail.Label} {detail.Value}";
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
