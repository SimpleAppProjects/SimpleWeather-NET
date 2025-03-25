using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace SimpleWeather.NET.Converters
{
    public partial class GridLengthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (double.TryParse(value?.ToString(), out double numberValue))
            {
                return new GridLength(numberValue, GridUnitType.Pixel);
            }

            return GridLength.Auto;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}